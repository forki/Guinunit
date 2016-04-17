# Guinunit

User interface for NUnit 3 test runner.

## Building

Guinunit uses [Paket](http://fsprojects.github.io/Paket) for dependency management and requires
some advanced features from 3.0 version. Until 3.0 becomes stable version, it's necessary to
bootstrap beta version of paket.

    > paket.bootstrapper.exe prerelease

Building visual studio solution should download correct version automatically (assuming that wrong
version is not already present inside `.paket` folder).

## Maintainers

* [@janno-p](https://github.com/janno-p)
* [@jaxx](https://github.com/jaxx)
