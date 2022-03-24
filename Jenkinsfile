#!/usr/bin/env groovy

REPO_NAME = 'cms-transforms-adidas'
TEMPDIR = env.TMPDIR ?: '/var/lib/jenkins/tmp'
VENV = 'virtualenv'

@Library('monetate-jenkins-pipeline')
import org.monetate.Slack
def slack = new Slack(steps, REPO_NAME)

slack.success(this, ':pipeline: Pipeline started')

node('node-v8') {
    try {
		def venvDir = "${TEMPDIR}/${env.BUILD_TAG}"

		stage('Checkout source') {
			checkout scm
		}

		withEnv(["PATH=${venvDir}/bin:${PATH}"]) {
			stage('Install requirements') {
				sh 'sh build.sh'
			}
			stage('Run tests') {
				sh 'make test-dotnet'
			}
	    }
	    currentBuild.result = result.SUCCESS
	    if (env.BRANCH_NAME == 'release') {
			sh 'make nuget-pack'
			sh 'pushd cms-transforms-c-sharp/CmsTransformLibrary && nuget push CmsTransformLibrary.1.0.0.nupkg -Source https://monetate.jfrog.io/artifactory/api/nuget/v3/dotnet-local -ApiKey U8Nj5klhb0iDlbI0ilbc2js8QF7zY68kqNSusutamG/z+gVfk86Y9p4KYAQYpxbadvVoBFU41F8pnr0K+1miZwjZkEAw7rSIATS3o0g2hqZzvRsatro5mQEVWF4OHSC3jhvauxTHW+Bhs9tvefN+3De6FW2E3v4Q0G+PPBlt/y8kSGeSTBvWsh1S/cp1jdWpCJ7VJCMYBrYa7l5D+lTjstO5LDn7FQaeX9s8COVTeIPugVAlqCjRQrLQyQkTQvB/e/FO9WRGkgN5PrjfYjxF7mKWtsTqGqrl1nzPJhfEiQvHBgmcfWpN1nG+MoF20Up4cDTajzcz60WRiU1/FR077UgjMIWOLQTnvYQg/nQv21RNafXQVoeoSC4tnJ2kD+3W && popd'
	    }
	} catch (err) {
		echo "Error starting jenkinsfile: ${err}"
    } finally {
        if (currentBuild.result == null) {
        currentBuild.result = result.FAILURE
        echo "Build result was null.  Defaulted to FAILURE."
        }
    }

    slack.currentResult(this, ':pipeline: Pipeline finished')
}