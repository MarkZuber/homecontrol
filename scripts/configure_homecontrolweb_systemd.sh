#!/bin/sh

# -- start first time only run

# sudo useradd -s /sbin/nologin homecontroluser
# sudo usermod -a -G users homecontroluser

# make directory for service to live in
# sudo mkdir /var/homecontrolwebsvc

# -- end first time only run

sudo systemctl stop homecontrolweb.service

sudo cp /home/pi/homecontrolweb_staging/homecontrolweb.service /etc/systemd/system/homecontrolweb.service
sudo cp -R /home/pi/homecontrolweb_staging/* /var/homecontrolwebsvc
sudo chown -R homecontroluser:homecontroluser /var/homecontrolwebsvc

sudo systemctl daemon-reload

sudo systemctl enable homecontrolweb.service
sudo systemctl start homecontrolweb.service
sudo systemctl status homecontrolweb.service
