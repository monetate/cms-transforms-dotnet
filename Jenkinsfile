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
    	withDotNet {
			def venvDir = "${TEMPDIR}/${env.BUILD_TAG}"

			stage('Checkout source') {
				checkout scm
			}


			stage('Create virtualenv') {
				sh "${VENV} ${venvDir}"
			}

			withEnv(["PATH=${venvDir}/bin:${PATH}"]) {
				try {
					stage('Run tests') {
						sh 'make test-dotnet'
					}
				} finally {
				}
			}
	    }
	    currentBuild.result = result.SUCCESS
	    if (env.BRANCH_NAME == 'master') {
			def tag = ''
			stage('Checkout source') {
				checkout([
					$class: 'GitSCM',
					branches: scm.branches,
					doGenerateSubmoduleConfigurations: scm.doGenerateSubmoduleConfigurations,
					extensions: [[$class: 'CloneOption', noTags: false, shallow: false, depth: 0, reference: '']],
					userRemoteConfigs: scm.userRemoteConfigs,
				])
			}

			try {
				tag = sh(returnStdout: true, script: 'git describe --tags --exact-match HEAD').trim()
			} catch (err) {
				echo "Could not find a tag on this commit: ${err}"
			} finally {
				echo "tag: ${tag}"
			}

			if (tag) {
				try {
					withEnv(["PATH=${venvDir}/bin:${PATH}"]) {
						stage('Pack artifact') {
							sh "make nuget-pack"
						}
						stage('Push artifact') {
							sh "pushd cms-transforms-c-sharp/CmsTransformLibrary && nuget push CmsTransformLibrary.${tag}.nupkg -Source ArtifactoryNugetV3 && popd"
						}
					}
				} catch (err) {
					currentBuild.result = result.FAILURE
					echo "Error packing and uploading artifact: ${err}"
				}
			}
	    }

    } finally {
        if (currentBuild.result == null) {
        currentBuild.result = result.FAILURE
        echo "Build result was null.  Defaulted to FAILURE."
        }
    }

    slack.currentResult(this, ':pipeline: Pipeline finished')
}