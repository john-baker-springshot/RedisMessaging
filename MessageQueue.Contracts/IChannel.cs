using System;
using System.Collections.Generic;
using MessageQueue.Contracts.Advices;
using MessageQueue.Contracts.Consumer;

namespace MessageQueue.Contracts
{
  public interface IChannel: IDisposable
  {
    /// <summary>
    /// Unique Identifier of the Channel
    /// </summary>
    string Id { get; }
    /// <summary>
    /// Default case of error handling for the Channel
    /// </summary>
    IErrorHandler DefaultErrorHandler { get; }
    /// <summary>
    /// Queue on which to receive messages from
    /// </summary>
    IQueue MessageQueue { get; }
    /// <summary>
    /// Queue on which to place messages not intended for this Channel
    /// </summary>
    IQueue PoisonQueue { get; }
    /// <summary>
    /// Queue on which to place errored messages we do not wish to retry/requeue
    /// </summary>
    IQueue DeadLetterQueue { get; }
    /// <summary>
    /// Message converter to convert the message to the appropriate type
    /// </summary>
    IMessageConverter MessageConverter { get; }

    /// <summary>
    /// List of Listeners subscribed to this Channel
    /// </summary>
    IEnumerable<IListener> Listeners { get; }
    /// <summary>
    /// Generic method for listening/subscribing to the MessageQueue
    /// </summary>
    /// <returns></returns>
    void Subscribe();
    /// <summary>
    /// Returns true if currently subscribed to MessageQueue
    /// </summary>
    bool IsSubscribed { get; }
    /// <summary>
    /// List of Advice error handlers for determining specific error handling scenarios
    /// </summary>
    IEnumerable<IAdvice> ErrorAdvice { get; }
    /// <summary>
    /// Number of instances of the channel you wish to spawn on initialization
    /// </summary>
    int Count { get; }
  }
}
