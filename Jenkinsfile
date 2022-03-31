#!/usr/bin/env groovy

REPO_NAME = 'cms-transforms-dotnet'
TEMPDIR = env.TMPDIR ?: '/var/lib/jenkins/tmp'
VENV = 'virtualenv'

@Library('monetate-jenkins-pipeline')
import org.monetate.Slack
def slack = new Slack(steps, REPO_NAME)

slack.success(this, ':pipeline: Pipeline started')

pipeline {
    agent { label "node-v8" }
    stages {
        stage('Checkout source') {
            steps {
                checkout scm
            }
        }

        stage('Tests') {
        	agent {
				dockerfile {
					label 'docker'
				}
			}
            steps {
                script {
					stage('Test') {
						sh 'dotnet test /cms-transforms-c-sharp/CmsTransformTests/'
					}
                }
            }
        }

        stage('Artifact: tag and publish') {
            when {
                branch 'release';
            }
            steps {
                sh "make nuget-pack"
                sh "pushd cms-transforms-c-sharp/CmsTransformLibrary && nuget push CmsTransformLibrary.1.0.0.nupkg -Source https://monetate.jfrog.io/artifactory/api/nuget/v3/dotnet-local -ApiKey U8Nj5klhb0iDlbI0ilbc2js8QF7zY68kqNSusutamG/z+gVfk86Y9p4KYAQYpxbadvVoBFU41F8pnr0K+1miZwjZkEAw7rSIATS3o0g2hqZzvRsatro5mQEVWF4OHSC3jhvauxTHW+Bhs9tvefN+3De6FW2E3v4Q0G+PPBlt/y8kSGeSTBvWsh1S/cp1jdWpCJ7VJCMYBrYa7l5D+lTjstO5LDn7FQaeX9s8COVTeIPugVAlqCjRQrLQyQkTQvB/e/FO9WRGkgN5PrjfYjxF7mKWtsTqGqrl1nzPJhfEiQvHBgmcfWpN1nG+MoF20Up4cDTajzcz60WRiU1/FR077UgjMIWOLQTnvYQg/nQv21RNafXQVoeoSC4tnJ2kD+3W && popd"
            }
        }
    }
}