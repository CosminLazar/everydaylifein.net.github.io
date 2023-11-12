---
layout: post
title: Ambient lighting controlled by Siri
date: 2017-06-18 17:45:48.000000000 +00:00
type: post
parent_id: '0'
published: true
password: ''
status: publish
categories:
- IoT
tags:
- ambient light
- arduino
- esp01
- esp8266
- home automation
- HomeKit
- iot
- LED strip
- MQTT
- Siri
- WS2812B
meta:
  _edit_last: '1'
  _yoast_wpseo_content_score: '30'
  _yoast_wpseo_primary_category: '55'
  _publicize_twitter_user: "@CosminLazar"
  _yoast_wpseo_focuskw_text_input: ambient light
  _yoast_wpseo_focuskw: ambient light
  _yoast_wpseo_linkdex: '79'
  _yoast_wpseo_metadesc: Design of an ambient light system using Siri, HomeKit, LED
    strip, MQTT and touch sensor
  _wpas_done_all: '1'
  enclosure: "https://everydaylifein.net/wp-content/uploads/2017/06/AmbientLights_demo.webm\r\n8080238\r\nvideo/webm\r\n"
  _wpas_skip_5526398: '1'
  _wpas_skip_5526404: '1'
  _wpas_skip_6755022: '1'
  _wpas_skip_7497947: '1'
author:
  login: admin
  email: cosminconstantinlazar@gmail.com
  display_name: Cosmin Lazar
  first_name: Cosmin
  last_name: Lazar
permalink: "/iot/ambient-lights-controlled-by-siri.html"
---
In this post, I will walk you through the moving parts required to build an ambient light system. The light will be controlled by any of the following:

- Siri (via HomeKit) - voice activation and configuration (power state, brightness, colour)
- HomeKit - software activation and configuration (power state, brightness, colour)
- Touch sensor - hardware activation and configuration (power state, brightness)

This is not a step-by-step guide but rather a description of the components involved and how the whole thing is put together.

## Architecture

The image below describes the overall architecture depicting the MQTT server as the central integration point.

The communication pattern is designed to adhere to the MQTT Smart Home Architecture, described [here](https://github.com/mqtt-smarthome/mqtt-smarthome).

{% figure caption:"Ambient lights architecture" %}
![Architecture diagram illustrating the ambient light system]({{ site.baseurl }}/assets/2017/06/LEDControllerSetup-300x254.png)
{% endfigure %}


## Hardware

- Arduino Uno - [buy on ebay](https://rover.ebay.com/rover/1/711-53200-19255-0/1?icep_id=114&ipn=icep&toolid=20004&campid=5338101636&mpre=http%3A%2F%2Fwww.ebay.com%2Fitm%2FARDUINO-UNO-R3-ATmega328P-ATmega16U2-Development-Board-with-USB-Cable-%2F292146680691%3Fhash%3Ditem44054c9373%3Ag%3A2I4AAOSwjk9ZPG3t)
- ESP8266 - ESP01 - [buy on ebay](https://rover.ebay.com/rover/1/711-53200-19255-0/1?icep_id=114&ipn=icep&toolid=20004&campid=5338101636&mpre=http%3A%2F%2Fwww.ebay.com%2Fitm%2F1PCS-ESP8266-ESP-01-WIFI-Wireless-Transceiver-Send-Receive-LWIP-AP-STA-%2F171907296150%3Fhash%3Ditem2806792b96%3Ag%3AX1cAAOSwk5FUt8Sq)
- LM1117T 3.3v voltage regulator - [buy on ebay](https://rover.ebay.com/rover/1/711-53200-19255-0/1?icep_id=114&ipn=icep&toolid=20004&campid=5338101636&mpre=http%3A%2F%2Fwww.ebay.com%2Fitm%2F10pcs-LM1117T-3-3-LM1117T-LD1117-3-3V-TO-220-%2F280945323105%3Fhash%3Ditem4169a56c61%3Am%3AmoLoOrhCzSSIY-_tFzL-6RA)
- Beefy 5v power supply - [buy on ebay](https://rover.ebay.com/rover/1/711-53200-19255-0/1?icep_id=114&ipn=icep&toolid=20004&campid=5338101636&mpre=http%3A%2F%2Fwww.ebay.com%2Fitm%2FDC-5V-3A-6A-8A-10A-LED-Wall-Wart-POWER-SUPPLY-AC100-240V-Adapter-Charger-for-LED-%2F391652855380%3Fvar%3D%26hash%3Ditem5b30544e54%3Am%3AmW4oU93p83TC7ItwH7bj0zw) - I used the 10 amps version, but you might get away with less
- WS2812B LED strip - [buy on ebay](https://rover.ebay.com/rover/1/711-53200-19255-0/1?icep_id=114&ipn=icep&toolid=20004&campid=5338101636&mpre=http%3A%2F%2Fwww.ebay.com%2Fitm%2FWS2812B-5050-RGB-LED-Strip-5M-150-300-Leds-144-60LED-M-Individual-Addressable-5V-%2F371432213255%3Fvar%3D%26hash%3Ditem567b15cb07%3Am%3AmtQ859zLUV_msJ6iSTwfRDg)
- Capacitive touch switch button - [buy on ebay](https://rover.ebay.com/rover/1/711-53200-19255-0/1?icep_id=114&ipn=icep&toolid=20004&campid=5338101636&mpre=http%3A%2F%2Fwww.ebay.com%2Fitm%2FHTTM-2-7V-6V-HTDS-SCR-Capacitive-Anti-interference-Touch-Switch-Button-Module-%2F252765441612%3Fvar%3D%26hash%3Ditem3ad9fe8e4c%3Am%3Amn7Qv1Gfk7ixyLG8kTgoA7Q)
- 2 x 1000uF capacitor - [buy on ebay](https://rover.ebay.com/rover/1/711-53200-19255-0/1?icep_id=114&ipn=icep&toolid=20004&campid=5338101636&mpre=http%3A%2F%2Fwww.ebay.com%2Fitm%2F10PCS-1000uF-25V-10mmx20mm-Radial-Electrolytic-Capacitors-%2F192096754699%3Fhash%3Ditem2cb9dbdc0b%3Ag%3Aud8AAOSw44BYltiT)
- Raspberry Pi - Optional - [buy on ebay](https://rover.ebay.com/rover/1/711-53200-19255-0/1?icep_id=114&ipn=icep&toolid=20004&campid=5338101636&mpre=http%3A%2F%2Fwww.ebay.com%2Fitm%2FRaspberry-Pi-3-Model-B-Quad-Core-1-2GHz-64bit-CPU-1GB-RAM-WiFi-Bluetooth-4-1-%2F141136387726%3Fhash%3Ditem20dc625e8e%3Ag%3AWkMAAOSwBnVW-foz)

The Arduino is the controlling unit and uses the ESP8266 chip to achieve wireless connectivity. There will be only one power supply (5v 10a) powering the Arduino, ESP8266, and the LED strip. Therefore, a 3.3v voltage regulator is necessary for the ESP8266 chip, and a capacitor should be used together with the regulator to smooth out any ripples.

The WS2812B strip supports addressing every LED individually, however, this functionality was not used and the strip is controlled as a whole. The strip is by far the most power-hungry of all components and special care should be taken to pair it with a suitable power supply.  Keep in mind that each LED is actually composed out of 3 colored LEDs (red, green, blue) that draw up to 60 milliamps at maximum brightness white.

The capacitive touch switch is used to manually turn the strip on or off, or to adjust the brightness level. A short sensor touch will cause the power to be toggled on/off while keeping the sensor touched will cause the brightness to gradually increase.

The Raspberry Pi does not play an active role in the hardware setup. It is used to host various software programs and can easily be replaced by a desktop, laptop, or NAS.

## Software

Controlling the ambient light system is performed by a suite of software:

- [LEDStripController](https://github.com/CosminLazar/LEDStripController) - runs on the Arduino and is responsible for communicating with the LED strips, publishing and responding to MQTT messages, and responding to the touch sensor inputs.
- [Espduino](https://github.com/tuanpmt/espduino) - firmware and software for the ESP01 chip to avoid working with AT commands.
- [Mosquitto](https://github.com/eclipse/mosquitto) - MQTT server, running on the Raspberry Pi.
- [Homekit2mqtt](https://github.com/hobbyquaker/homekit2mqtt) - HomeKit to MQTT bridge, running on the Raspberry Pi.

If you are going for a similar setup, keep in mind that Espduino is discontinued! I recommend using an ESP12 instead of the Arduino+Esp01 combination.

In case of network loss, ESP01 automatically tries to reconnect to the wifi, however, there are times when the chip stops responding entirely. To mitigate this issue, a "watchdog" was set in place to hard reboot the chip (power cycle) when it stops responding.

In the general usage pattern, a burst of messages are sent to the controlling pair (ESP01+Arduino) and processing those messages is prioritised over updating the hardware (actually turning the LED strip on and configuring each LED according to the chosen colour and brightness). This debounce is controlled by the HARDWARE_UPDATE_DEBOUNCE parameter.

Touching the touch sensor for a short period of time triggers a power toggle, while keeping the sensor touched causes it to cycle the brightness in steps. It takes 10 steps to go from the lowest to the highest brightness level.

## Assembly

Here are a few pictures from the assembly process:

{% figure caption:"Assembly process #1" %}
![Assembly process #1]({{ site.baseurl }}/assets/2017/06/IMG_0018.jpg)
{% endfigure %}

{% figure caption:"Assembly process #2" %}
![Assembly process #2]({{ site.baseurl }}/assets/2017/06/IMG_0019.jpg)
{% endfigure %}

{% figure caption:"Assembly process #3" %}
![Assembly process #3]({{ site.baseurl }}/assets/2017/06/IMG_0129.jpg)
{% endfigure %}

## Demo

[![Ambient lights demo video]({{ site.baseurl }}/2017/06/AmbientLights_demo.mp4)](https://everydaylifein.net/wp-content/uploads/2017/06/AmbientLights_demo.mp4)