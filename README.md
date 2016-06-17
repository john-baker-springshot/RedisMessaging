## Synopsis

RedisMessaging is a lightweight Redis Messaging Queue framework utilizing StackExchange.Redis for interaction with redis and spring.core for DI/configuration. 

Inspired by AMQP messaging frameworks/infrastructures, RedisMessaging contains several out of the box features, akin to any Messaging infratructure, such as Poison/DeadLetter Queues, a transient Processing Queue per Listener to reduce the likelyhood of lost messages, a Sentinel class that scans all Processing Queues for abandoned messages eligible for Requeuing, and customizable error handling via advice chaining mechanisms.

## Motivation

Wanting a more feature-rich Message Queue framework similar to Spring.AMQP.RabbitMQ without sacrificing the speed and ease of use of Redis, we took the parts of Spring's RabbitMQ we liked (namely the spring-based configuration and infratructure principles) and the most supported open source .Net Redis Framework (StackExchange.Redis) and combined the two into a lightweight messaging framework.

## Code Example

The most important part of getting started with RedisMessaging is to set up your configuration file based on your needs. The config format follows spring.net object definition conventions. We have put a nice wrapper around it in order to make it clear and concise.

```
<!--Basic Consumer Configuration-->
<redis:connection id="myConnection"
				endpoints="localhost:6379"
				password="password"
				defaultDatabase="1"
				abortConnect="true"
				allowAdmin="true"
				connectRetry="5" />

<redis:sentinel id="sentinel" connection="myConnection" messageTimeout="60" interval="1000" />

<redis:container id="myContainer"
			   connection="myConnection"
			   enableSentinel="true"
			   sentinel="sentinel">

<redis:channels>
  <redis:channel id="myChannel" concurrency="5">

	<redis:listeners>
	  <redis:listener handlerType="RedisMessaging.Tests.ConsumerTests.TestMessageHandler, RedisMessaging.Tests" handlerMethod="HandleMessage" typeKey="Event" />
	</redis:listeners>

	<redis:queues>
	  <redis:messageQueue name="myRedisQ" />
	  <redis:deadLetterQueue name="myDeadLetterQ" />
	  <redis:poisonQueue name="myPoisonQ" />
	</redis:queues>

	<redis:messageConverter createMessageIds="true">
	  <redis:typeMapper>
		<redis:knownType key="Basic" type="RedisMessaging.BasicMessage, RedisMessaging" />
		<redis:knownType key="Event" type="Consumer.Event, Consumer" />
	  </redis:typeMapper>
	</redis:messageConverter>

	<redis:adviceChain>
	  <redis:advice retryOnFail="true" exceptionType="TimeoutException" adviceType="TimedRetry" retryCount="8" retryInterval="30" />
	  <redis:advice id="advice2" retryOnFail="true" exceptionType="System.Data.SqlClient.SqlException" adviceType="RetryRequeue" />
	</redis:adviceChain>

  </redis:channel>
</redis:channels>

</redis:container>

<!--Producer Configuration-->
<redis:producer id="myProducer" connection="myConnection" queue="myQueue" />

```

And so on, you can look at the Producer.config or Consumer.config files in the source for a clearer picture on how everything works, more documentation on the subject to come

Once configured, all you need to do is fetch the Producer, using Spring IoC, defined in the configuration and start .Publish - ing messages. For the Consumer, only the Container need be fetched, after which Container.Init() will begin the message consuming process.

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