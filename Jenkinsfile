#!/usr/bin/env groovy
stage("Windows") {
  node('windows') {
    checkout poll: false, changelog: false, scm: scm
    bat ("Protobuild.exe --upgrade-all")
    bat ('Protobuild.exe --automated-build')
    stash includes: '*.nupkg', name: 'windows-packages'
  }
}

stage("Mac") {
  node('mac') {
    checkout poll: false, changelog: false, scm: scm
    sh ("mono Protobuild.exe --upgrade-all MacOS")
    sh ("mono Protobuild.exe --upgrade-all Android")
    sh ("mono Protobuild.exe --upgrade-all iOS")
    sh ("mono Protobuild.exe --automated-build")
    stash includes: '*.nupkg', name: 'mac-packages'
  }
}

stage("Linux") {
  node('linux') {
    checkout poll: true, changelog: true, scm: scm
    sh ("mono Protobuild.exe --upgrade-all")
    sh ("mono Protobuild.exe --automated-build")
    stash includes: '*.nupkg', name: 'linux-packages'
  }
}

stage("Unified") {
  node('windows') {
    // Ensure a seperate working directory to the normal linux node above.
    ws {
      checkout poll: true, changelog: true, scm: scm
      if (fileExists('unified.build')) {
        unstash 'windows-packages'
        unstash 'mac-packages'
        unstash 'linux-packages'
        bat ("Protobuild.exe --automated-build unified.build")
      }
    }
  }
}