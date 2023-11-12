---
layout: post
title: Arduino - DHT22 temperature and humidity sensor driver
date: 2016-03-22 22:36:06.000000000 +00:00
type: post
parent_id: '0'
published: true
password: ''
status: publish
categories:
- IoT
tags:
- arduino
- dht
- dht22
- humidity sensor
- iot
- temperature sensor
meta:
  _edit_last: '1'
  _yoast_wpseo_primary_category: '55'
  _publicize_twitter_user: "@CosminLazar"
  _yoast_wpseo_focuskw_text_input: Arduino DHT driver
  _yoast_wpseo_focuskw: Arduino DHT driver
  _yoast_wpseo_linkdex: '68'
  _yoast_wpseo_metadesc: Connecting the DHT22 temperature and relative humidity sensor
    to Arduino. Step by step tutorial accompanied by the required driver.
  _wpas_mess: Arduino - DHT22 temperature and humidity sensor driver
  _wpas_done_all: '1'
  _yoast_wpseo_content_score: '60'
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
permalink: "/iot/arduino-dht-sensor-protocol.html"
---
DHT22/AM2302 is an inexpensive temperature and relative humidity sensor. The sensor, which can be bought on [ebay](https://rover.ebay.com/rover/1/711-53200-19255-0/1?icep_id=114&ipn=icep&toolid=20004&campid=5338101636&mpre=http%3A%2F%2Fwww.ebay.com%2Fitm%2FDHT22-AM2302-Digital-Temperature-And-Humidity-Sensor-Replace-SHT11-SHT15-%2F381605063274%3Fhash%3Ditem58d96f2a6a%3Ag%3AEYwAAOSwajVUNpxD), implements a digital communication protocol by pulling high or low on a data line. The protocol is described [here](https://github.com/CosminLazar/Arduino-DHTSensor/blob/master/literature/Digital%20humidity%20and%20temperature%20sensor%20AM2302.pdf) and it has three states:

1. transmission start - initiated by a host and acknowledged by the sensor
2. data transfer - binary transmission
3. transmission end - performed by the sensor

# Setup

The communication protocol instructs that the data line needs to always be kept high using a pull up resistor (this is because the sensor will pull low on the line to transmit data). However, in this specific implementation, an external pull up resistor is not needed because the driver makes usage of the internal pull up resistor of the Arduino pin.

The cables need to be connected as follows:

- pin1 = VDD
- pin2 = data
- pin3 = not used
- pin4 = GND

[![arduino_dht22]({{ site.baseurl }}/assets/2016/03/arduino_dht22-300x249.png)]({{ site.baseurl }}/assets/2016/03/arduino_dht22.png)

# Usage

- First you need to download the driver source code from [here](https://github.com/CosminLazar/Arduino-DHTSensor/archive/master.zip) or you can browse it [here](https://github.com/CosminLazar/Arduino-DHTSensor).
- Next, copy the unzipped contents to your Arduino libraries directory in a subdirectory named DHTSensor.
- Restart your IDE

You are now ready to do a reading

{% gist 2f35506f9f1bae44cbe0 DHT22ArduinoSample.cpp %}

As it can be observed from the example, the temperature of the measurement result can be interpreted in Celsius, Fahrenheit or Kelvin degrees, while the humidity is percentual.

Furthermore, the sensor can also be powered by a digital pin using the following construct

{% gist 2f35506f9f1bae44cbe0 PowerByPinConstructor.cpp %}

Keep in mind that a sensor measurement will take around a second and during this time the execution is not yielded back (your board will stop processing anything else). The one second is due to how the sensor is engineered to communicate and the non-yielding execution is due to the implementation of the driver.

# Disclaimer

Implementing the driver was performed as an educational project and it comes with no guarantees. You are free to use and modify it in any way it makes sense for you.
