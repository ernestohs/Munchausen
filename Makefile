.PHONY: build test clean

build: clean

	xbuild /p:OS=mono /p:Configuration=Release Munchausen.sln

test: build

	nunit-console ./Munchausen.Tests/bin/Release/Munchausen.Tests.dll

clean:

	xbuild /target:Clean Munchausen.sln

	find . -type d -name 'bin' -o -name 'obj' | xargs rm -rvf 
