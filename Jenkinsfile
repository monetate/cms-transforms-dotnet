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
					stage('Test Cms Transforms') {
						sh 'pushd cms-transforms-c-sharp/CmsTransformTests && dotnet test && popd'
					}
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
				def versionPrefix =  sh(returnStdout: true, script: 'grep -o (?<=<Version>).*(?=</Version>) ./cms-transforms-c-sharp/CmsTransformLibrary/CmsTransformLibrary.csproj')
				sh "echo ${versionPrefix}"
				sh "pushd cms-transforms-c-sharp/CmsTransformLibrary"
                sh "nuget pack CmsTransformLibrary.csproj"
                def key = sh(aws s3 cp s3://secret-monetate-dev/artifactory/monetate.jfrog.io/dotnet-local/dotnet-local-pw -)
                sh "nuget push CmsTransformLibrary.${versionPrefix}.nupkg -Source https://monetate.jfrog.io/artifactory/api/nuget/v3/dotnet-local -ApiKey ${key}"
                sh "popd"
            }
        }
    }
}