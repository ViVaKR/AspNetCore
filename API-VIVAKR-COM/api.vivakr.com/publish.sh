#! /usr/bin/env zsh

rm -rf ~/WebServer/com.vivakr/api/ &&
    echo "Removing is done." &&
    dotnet publish --configuration Release --output ~/WebServer/com.vivakr/api/ &&
    echo "Publishing is done."
