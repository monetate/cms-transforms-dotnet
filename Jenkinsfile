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
					sh 'cd cms-transforms-c-sharp/CmsTransformTests && dotnet test && cd ../..'
                }
            }
        }

        stage('Artifact: tag and publish') {
        	agent {
				dockerfile {
					label 'docker'
				}
			}
//             when {
//                 branch 'release';
//             }
            steps {
            	script {
					def versionPrefix =  sh(returnStdout: true, script: "cd cms-transforms-c-sharp/CmsTransformLibrary && grep '<Version>' < CmsTransformLibrary.csproj | sed 's/.*<Version>\\(.*\\)<\\/Version>/\\1/'").trim()
					sh "echo ${versionPrefix}"
					sh "cd cms-transforms-c-sharp/CmsTransformLibrary && dotnet pack CmsTransformLibrary.csproj"
					def key = sh(returnStdout: true, script: "aws s3 cp s3://secret-monetate-dev/artifactory/monetate.jfrog.io/dotnet-local/dotnet-local-pw -").trim()
					def src = "CmsTransformLibrary.${versionPrefix}.nupkg"
					sh "echo ${src}"
					sh "echo ${key}"
					sh "dotnet nuget push cms-transforms-c-sharp/CmsTransformLibrary/ -s ${src} -ss https://monetate.jfrog.io/artifactory/api/nuget/v3/dotnet-local -sk ${key}"
                }
            }
        }
    }
}