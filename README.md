BLEx11xLib
==========

A simple implementation of a Bluetooth LE Heart Rate sensor for the Bluegiga BLED112 dongle, using Windows WPF and BGLib.


This is a port of the BLED112 BGScript heart rate monitor sample by Jeff Rowberg (https://bluegiga.zendesk.com/entries/22810076--HOW-TO-Run-a-BGScript-application-on-the-BLED112-USB-dongle). It's written in C# using Jeff's port of BGLib, and is a Windows WPF desktop app. It has the important addition of being bondable - this is needed for it to be used with Windows 8.1 and Windows Phone 8.1. If your computer has BT 4.0 in addition to the BLED112 dongle, you can bond to the program on one PC.

Requirements
------------

- A Bluegiga BLED112 dongle
- The Bluegiga GUI
- Visual Studio 2013
- Silverlight & WPF Dashboards & Gauges. Download from http://dashboarding.codeplex.com. These aren't strictly necessary - I just used them for the LED light controls.


Preparing the Module
--------------------

To run the code, you'll need to flash the dongle with the code provided in the 'BLED112 Heart Rate BGLib' folder. This provides the necessary GATT Attributes.

Running the code
----------------

Once the solution is running, select the COM port your BLED112 is connected to, then press Start Device. The module should be in advertising mode, and you should see it on your other BTLE device. Once connected, pressing Start Outputting Data will simply cycle the heart rate measurement from 0-256bpm. And that's all there is to it!

Notes & discoveries
-------------------

If you're planning on using this code as the basis for your own project, a couple of pointers:
- The event handlers run on a different thread to the UI. If you need to update the UI in an event handler, you'll need to use Dispatcher.Invoke(), or an exception will be thrown.
- BLECommandAttributesWrite used the handle number to identify the attribute you're writing to. A map of the attribute ID to handle number is contained in the attributes.txt file in the 'BLED112 Heart Rate BGLib' folder. 
