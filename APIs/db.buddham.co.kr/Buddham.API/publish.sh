#! /usr/bin/env zsh

rm -rf ~/WebServer/kr.co.buddham/db/ &&
    echo "Removing is done." &&
    dotnet publish --configuration Release --output ~/WebServer/kr.co.buddham/db/ &&
    echo "Publishing is done."
