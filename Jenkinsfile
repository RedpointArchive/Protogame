#!/usr/bin/env groovy
stage("Windows") {
  node('windows') {
    checkout poll: false, changelog: false, scm: scm
    bat ("git clean -xdff")
    bat ("Protobuild.exe --upgrade-all")
    try {
      bat ('Protobuild.exe --automated-build')
      stash includes: '*.nupkg', name: 'windows-packages'
    } finally {
      publishHTML([allowMissing: true, alwaysLinkToLastBuild: true, keepAll: true, reportDir: 'Protogame.Tests/bin/Windows/AnyCPU/Debug/VisualHTML', reportFiles: 'index.html', reportName: 'Visual Test Report - Windows'])
    }
  }
}

stage("Mac") {
  node('mac') {
    checkout poll: false, changelog: false, scm: scm
    sh ("git clean -xdff")
    sh ("mono Protobuild.exe --upgrade-all MacOS")
    sh ("mono Protobuild.exe --upgrade-all Android")
    sh ("mono Protobuild.exe --upgrade-all iOS")
    try {
      sh ("mono Protobuild.exe --automated-build")
      stash includes: '*.nupkg', name: 'mac-packages'
    } finally {
      publishHTML([allowMissing: true, alwaysLinkToLastBuild: true, keepAll: true, reportDir: 'Protogame.Tests/bin/MacOS/AnyCPU/Debug/VisualHTML', reportFiles: 'index.html', reportName: 'Visual Test Report - macOS'])
    }
  }
}

stage("Linux") {
  node('linux') {
    checkout poll: true, changelog: true, scm: scm
    sh ("git clean -xdff")
    sh ("mono Protobuild.exe --upgrade-all")
    try {
      sh ("mono Protobuild.exe --automated-build")
      stash includes: '*.nupkg', name: 'linux-packages'
    } finally {
      publishHTML([allowMissing: true, alwaysLinkToLastBuild: true, keepAll: true, reportDir: 'Protogame.Tests/bin/Linux/AnyCPU/Debug/VisualHTML', reportFiles: 'index.html', reportName: 'Visual Test Report - Linux'])
    }
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