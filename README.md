## Synopsis

RedisMessaging is a lightweight Redis Messaging Queue framework utilizing StackExchange.Redis for interaction with redis and spring.core for DI/configuration. 

Inspired by RabbitMQ, RedisMessaging contains many out of the box features such as Poison/DeadLetter Queues, a transient Processing Queue per Listener to reduce the likelyhood of lost messages, a Sentinel class that scans all Processing Queues for abandoned messages eligible for Requeuing, and customizable error handling via RetryRequeue and TimedRetry mechanisms.

## Motivation

Wanting a more feature-rich Message Queue closer to RabbitMQ without sacrificing the speed and ease of use of Redis, we took the parts of RabbitMQ we liked (namely the spring-based configuration and infratructure principles) and the most supported open source .Net Redis Framework (StackExchange.Redis) and combined the two into a lightweight framework.

## Code Example

The most important part of getting started with RedisMessaging is to set up your configuration file based on your needs. The config format follows spring.net object definition conventions

<!--Basic Configuration-->
<object name="MyConnection"
  type="RedisMessaging.RedisConnection, RedisMessaging">
<constructor-arg name="connectionString" value="localhost:6379"/>
</object>

<object name="MyMessageQueue"
  type="RedisMessaging.RedisQueue, RedisMessaging">
<constructor-arg name="Name" value="MessageQueue"/>
<constructor-arg name="TTL" value="0"/>
</object>

<!--Producer Configuration-->
<object name="MyProducer" type="RedisMessaging.Producer.RedisProducer, RedisMessaging">
<constructor-arg name="connection" ref="MyConnection"/>
<constructor-arg name="queue" ref="MyMessageQueue"/>
</object>

And so on, you can look at the Implementation.xml files in the source for a clearer picture on how everything works, more documentation on the subject to come

Once configured, all you need to do is instantiate the Producer defined in the configuration and start .Publish - ing messages. For the Producer, only the Container object need be instantiated, after which Container.Init() will begin the message consuming process.

- More to come

## Installation

- More to come

## API Reference

All interfaces are visible under MessageQueue.Contracts.

## Tests

Tests are included in the solution for basic scenarios. Feel free to run them against your own configuration to ensure all is working as it should.

## Contributors

Primary Contributors: Kunjan Modi, John Baker. Comments/suggestions/pull requests welcome.

## License

This project is covered under the MIT Open Source License, see LICENSE.txt for more information.