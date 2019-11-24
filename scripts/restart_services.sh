#!/bin/bash

echo "Restarting homecontrolweb..."
ssh pi@192.168.2.203 'bash -s' < ./scripts/configure_homecontrolweb_systemd.sh
echo "Restarting streamdeck..."
ssh pi@192.168.2.203 'bash -s' < ./scripts/configure_streamdeck_systemd.sh
