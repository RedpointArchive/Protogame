#!/usr/bin/env groovy
stage("Build") {
  parallel (
    "Windows" : {
      node('windows') {
        checkout poll: false, changelog: false, scm: scm
        bat ("Protobuild.exe --upgrade-all")
        bat ('Protobuild.exe --automated-build')
        stash includes: '*.nupkg', name: 'windows-packages'
      }
    },
    "Mac": {
      node('mac') {
        checkout poll: false, changelog: false, scm: scm
        sh ("mono Protobuild.exe --upgrade-all MacOS")
        sh ("mono Protobuild.exe --upgrade-all Android")
        sh ("mono Protobuild.exe --upgrade-all iOS")
        sh ("mono Protobuild.exe --automated-build")
        stash includes: '*.nupkg', name: 'mac-packages'
      }
    },
    "Linux": {
      node('linux') {
        checkout poll: true, changelog: true, scm: scm
        sh ("bash -c 'Xorg -noreset +extension GLX +extension RANDR +extension RENDER -config /xorg.conf :2 & echo \"\$!\" > xorg.pid'")
        try {
          sh ("mono Protobuild.exe --upgrade-all")
          sh ("DISPLAY=:2 mono Protobuild.exe --automated-build")
          stash includes: '*.nupkg', name: 'linux-packages'
        } finally {
          sh ("if [ -e xorg.pid ]; then kill \$(<xorg.pid); fi")
        }
      }
    }
  )
}

stage("Package") {
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