# Iris

Infrastructure to forward messages from a queue-like system to browser clients connected over Websockets.

It supports a Cloud enviroment where there are multiple instances and they are load balanced by using a secondary dispatching subsystem over Redis (Pub/Sub).

Current implementation is based on:

- Messaging: NSQ (https://github.com/judwhite/NsqSharp)
- Cluster nodes communication: Redis

