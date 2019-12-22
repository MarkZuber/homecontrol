#!/bin/bash

ssh-keygen
ssh-copy-id pi@retropie

ssh-add -K ~/.ssh/id_rsa
