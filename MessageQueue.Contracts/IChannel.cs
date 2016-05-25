using System;
using System.Collections.Generic;
using MessageQueue.Contracts.Advices;
using MessageQueue.Contracts.Consumer;

namespace MessageQueue.Contracts
{
  public interface IChannel
  {
    /// <summary>
    /// Unique Identifier of the Channel
    /// </summary>
    string Id { get; set; }
    /// <summary>
    /// Default case of error handling for the Channel
    /// </summary>
    IErrorHandler DefaultErrorHandler { get; set; }
    /// <summary>
    /// Queue on which to receive messages from
    /// </summary>
    IQueue MessageQueue { get; set; }
    /// <summary>
    /// Queue on which to place messages not intended for this Channel
    /// </summary>
    IQueue PoisonQueue { get; set; }
    /// <summary>
    /// Queue on which to place errored messages we do not wish to retry/requeue
    /// </summary>
    IQueue DeadLetterQueue { get; set; }

    /// <summary>
    /// Type mapper used to store the "types" of messages this Channel is meant to process
    /// </summary>
    ITypeMapper TypeMapper { get; set; }

    /// <summary>
    /// List of Listeners subscribed to this Channel
    /// </summary>
    IList<IListener> Listeners { get; set; }
    /// <summary>
    /// Generic method for listening/subscribing to the MessageQueue
    /// </summary>
    /// <returns></returns>
    void Subscribe();
    /// <summary>
    /// Returns true if currently subscribed to MessageQueue
    /// </summary>
    bool IsSubscribed { get; set; }
    /// <summary>
    /// List of Advice error handlers for determining specific error handling scenarios
    /// </summary>
    IList<IAdvice<Exception>> ErrorAdvice { get; set; }
  }
}
