
# Introduction 
The goal of this project is to make a full IOT based aquarium monitor / controller.  This project is borrowing ideas or concepts from other similar open-source projects.  The inspiration originally came from [Reef-Pi](https://reef-pi.github.io/) and a lot of the technology inspiration came from [Submerged](https://github.com/jsiegmund/submerged) 

This project is built with .NET using Rasppberry PI's with a full Azure back-end.  

As you look through the code you can tell it's several years old.  I also haven't really worked on it for a long time.  You'll also see some less than professional merge names because I never really intended to make this public, it was more for me.  I have since decided to get back into technology and I want to have my github full of the various projects I have done.  

Couple notes about this project.  Making an aquarium controller is hard, like really really hard.  There's a reason that there's only a couple commercially available and one that has been around for a long time.  In fact there has been successful companies (eh hem Vertex) that wanted to get into the controller game and subsequently went out of business.

Probably one of the most useful parts of this are the wrappers for interacting with the Atlas Scientific modules.  If I get a random few hours, I would love to make a Nuget library and let people use it.  

##Description:

I would love to tell you I remember what everything does, but it would take me a while to study it and remember.  Generally speaking there was a science module that would read Temperature, PH, and Salinity.  There was a 'control' module that operated the relays and turned on and off outlets, either on a schedule or because of the readings the science module gave (ie turning off the heater when the tank got warm enough).  There was also a doser module that operated two EZO Pumps that would dose calcium and alkalinity.  

The end goal was there would be one "edge" module that would take in all the data from the other modules and people could have several tanks.  As an example, 3 tanks, 3 science modules, 1 edge module, and all the data would flow into one place.  My idea was a hobbyist with several tanks / or a local fish store would be able to piece a system together that could handle all of their systems in one place.  

# Hardware // 

1. Raspberry Pi 3/4 [Link](https://www.amazon.com/ELEMENT-Element14-Raspberry-Pi-Motherboard/dp/B07BDR5PDW/ref=sr_1_3?s=pc&ie=UTF8&qid=1537397089&sr=1-3&keywords=raspberry+pi+3+b%2B)
2. Raspberry Pi 7 Inch screen [Link](https://www.amazon.com/Raspberry-Pi-7-Touchscreen-Display/dp/B0153R2A9I/ref=sr_1_3?s=electronics&ie=UTF8&qid=1537397181&sr=1-3&keywords=raspberry+pi+screen)
3. Whitebox Labs Tentacle Shield (Not Currently Implemented, future development)[Link](https://www.whiteboxes.ch/shop/tentacle/?v=7516fd43adaa)
4. Atlas Scientific EZO Circuits (For all parameters you want to monitor)[Link](https://www.atlas-scientific.com/)
    - Required Circuits:
        - EZO PH
        - EZO Temperature
5. Sainsmart Solid State Relay [Link](https://www.amazon.com/gp/product/B006J4G45G/ref=oh_aui_detailpage_o00_s00?ie=UTF8&psc=1)
6. Sainsmart Mechanical Relay [Link](https://www.amazon.com/gp/product/B0057OC5O8/ref=oh_aui_detailpage_o00_s00?ie=UTF8&psc=1)


# Contribute
Looking for others to contribute to this project!  I am by no means a professional and would love the oppurtunity to have some other reef hobbyists and/or programmers help out. 

The code here is free to use, if you take this project and move the ball forward, I would love help you test, etc.  Just contact me!     

