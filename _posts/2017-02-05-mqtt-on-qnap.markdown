---
layout: post
title: MQTT on QNAP via Moquette
date: 2017-02-05 13:42:22.000000000 +00:00
type: post
parent_id: '0'
published: true
password: ''
status: publish
categories:
- IoT
tags:
- iot
- MQTT
- QNAP
meta:
  _edit_last: '1'
  _yoast_wpseo_primary_category: '55'
  _publicize_twitter_user: "@CosminLazar"
  _yoast_wpseo_focuskw_text_input: MQTT QNAP
  _yoast_wpseo_focuskw: MQTT QNAP
  _yoast_wpseo_linkdex: '56'
  _yoast_wpseo_content_score: '30'
  _wpas_done_all: '1'
author:
  login: admin
  email: cosminconstantinlazar@gmail.com
  display_name: Cosmin Lazar
  first_name: Cosmin
  last_name: Lazar
permalink: "/iot/mqtt-on-qnap.html"
---

Message Queuing Telemetry Transport (MQTT) is a lightweight pub/sub messaging protocol designed for minimal bandwidth and device resource requirements. This makes it ideal for IoT projects, where you generally have limited processing power and memory space ([read more about MQTT](http://mqtt.org/){:target="_blank"}).

There are several MQTT broker/server implementations out there ([hopefully exhaustive list](https://github.com/mqtt/mqtt.github.io/wiki/servers){:target="_blank"}), however most of then cannot be ran on my QNAP TS221 Network Attached Storage because it uses a custom version of Linux and it is run by an ARM processor.

On the other hand, [Moquette](https://github.com/andsel/moquette){:target="_blank"} is a MQTT broker written in Java, so I gave it a try.


1. Install the Java Runtime Environment for ARM - for some reason I could not find it in the App Center, so I had to download the package from [here](https://www.qnap.com/i/in/app_center/con_show.php?op=showone&internalName=JRE_ARM&version=8.65.0&down_1_name=TS-NASARM&jump_win=1){:target="_blank"} (dead link) and install it via the Install Manually button.
2. SSH into the QNAP ([guide](http://wiki.qnap.com/wiki/How_to_SSH_into_your_QNAP_device){:target="_blank"})
3. Follow the steps listed in the Moquette [repo](https://github.com/andsel/moquette){:target="_blank"}
    1. wget https://bintray.com/artifact/download/andsel/generic/distribution-0.8-bundle-tar.tar.gz --no-check-certificate
    2. tar xvfz distribution-0.8-bundle-tar.tar.gz
    3. By default Moquette uses port 8080 for WebSocket which is also used by the QNAP administration interface, so I changed the Moquette port to 9090 by navigating to /config and setting websocket_port to 9090 in moquette.conf
    4. navigate to /bin and run moquette.sh
4. By now you should have a working MQTT broker, you can test it with [this online tool](http://mitsuruog.github.io/what-mqtt/){:target="_blank"}
5. Optionally you can configure your moquette.sh to be run at startup

