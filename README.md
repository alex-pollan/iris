# websockets

Infrastructure to forward messages from a queue-like system to clients connected over Websockets.

It supports a Cloud enviroment where there are multiple instances and they are load balanced by using a secondary dispatching subsystem over Redis (Pub/Sub).


## How to test

1. Navigate to https://websocket-test.ausvdc02.pcf.dell.com/?customerSet=rc1111
2. Post to NSQ:

    POST/ http://dv1vmrowprem01.oldev.preol.dell.com:4151/pub?topic=HelloMessage

    ``{"customerSet": "rc1111", "text": "Hello Websockets"}``

    Make sure the property customerSet matches the value in the query string in point 1
    
3. You should see the text "Hello Websockets" in the browser.
4. Optionally open more tabs in the browser with different values for customerSet and try posting to NSQ with these values.
