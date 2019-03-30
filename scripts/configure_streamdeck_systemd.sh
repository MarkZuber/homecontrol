#!/bin/sh

# -- start first time only run

# need to run this command and reboot to configure security access to the HID to the users group
# sudo tee /etc/udev/rules.d/10-streamdeck.rules << EOF
# 	SUBSYSTEMS=="usb", ATTRS{idVendor}=="0fd9", ATTRS{idProduct}=="0060", GROUP="users"
# 	SUBSYSTEMS=="usb", ATTRS{idVendor}=="0fd9", ATTRS{idProduct}=="0063", GROUP="users"
# 	EOF

# sudo useradd -s /sbin/nologin streamdeckuser

# user must be in users group since that's where the security access is granted to
# sudo usermod -a -G users streamdeckuser

# make directory for service to live in
# sudo mkdir /var/streamdecksvc

# -- end first time only run

sudo systemctl stop streamdeck.service

sudo cp /home/pi/streamdeck_staging/streamdeck.service /etc/systemd/system/streamdeck.service
sudo cp -R /home/pi/streamdeck_staging/* /var/streamdecksvc
sudo chown -R streamdeckuser:streamdeckuser /var/streamdecksvc

sudo systemctl start streamdeck.service
sudo systemctl status streamdeck.service
