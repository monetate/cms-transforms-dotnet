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
    environment {
        DOTNET_CLI_HOME = "/tmp/DOTNET_CLI_HOME"
        ARTIFACTORY_USER = credentials('monetate-jenkins-artifactory-user')
        ARTIFACTORY_PW = credentials('monetate-jenkins-artifactory-password')
    }
    stages {
        stage('Checkout source') {
            steps {
                checkout scm
            }
        }

        stage('Run Tests') {
        	agent {
				dockerfile {
					label 'docker'
				}
			}
            steps {
                script {
					sh 'make dotnet-test'
                }
            }
        }

        stage('Artifact: tag and publish') {
        	agent {
				dockerfile {
					label 'docker'
				}
			}
            when {
                branch 'release';
            }
            steps {
            	script {
					def versionPrefix =  sh(returnStdout: true, script: "cd cms-transforms-c-sharp/CmsTransformLibrary && grep '<Version>' < CmsTransformLibrary.csproj | sed 's/.*<Version>\\(.*\\)<\\/Version>/\\1/'").trim()
					sh "echo Version being uploaded: ${versionPrefix}"
					sh "make dotnet-pack"
					sh 'chmod +x ./publish.sh'
					sh './publish.sh ${ARTIFACTORY_USER} ${ARTIFACTORY_PW}'
                }
            }
        }
    }
}