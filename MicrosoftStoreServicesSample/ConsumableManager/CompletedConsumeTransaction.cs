﻿//-----------------------------------------------------------------------------
// CompletedConsumeTransaction.cs
//
// Xbox Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License file under the project root for
// license information.
//-----------------------------------------------------------------------------

using Microsoft.StoreServices;
using System;
using System.ComponentModel.DataAnnotations;


namespace MicrosoftStoreServicesSample
{
    /// <summary>
    /// Object class to be added to our persistent database that will represent a
    /// consume transaction to lookup if the OrderId and OrderLineItemId are 
    /// present in a Clawback service result for refunded items.
    /// </summary>
    public class CompletedConsumeTransaction : ConsumeOrderTransactionContractV8
    {
        /// <summary>
        /// This is a Guid generated on creation to be a unique Id key
        /// in the database.  This is because multiple items could have
        /// the same trackingId or orderLineItemID if the consumable is
        /// setup to grant more than qty 1 when purchased in the store.
        /// </summary>
        [Key]
        public string DbKey { get; set; }

        public CompletedConsumeTransactionState TransactionStatus { get; set; }

        /// <summary>
        /// TrackingId used on the Consume request
        /// </summary>
        public string TrackingId { get; set; }

        /// <summary>
        /// Time that the consume was fulfilled or verified
        /// </summary>
        public DateTimeOffset ConsumeDate { get; set; }

        /// <summary>
        /// ProductId of the consumable product
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Unique Id used to identify the user in your service
        /// </summary>
        public string UserId { get; set; }

        public CompletedConsumeTransaction() { }
        
        /// <summary>
        /// Creates an item to add to the Clawback validation queue from a completed
        /// PendingConsumeRequest object
        /// </summary>
        /// <param name="request">Completed consume request info</param>
        public CompletedConsumeTransaction(string UserId, string ProductId, string TrackingId, ConsumeOrderTransactionContractV8 transaction)
        {
            this.TrackingId  = TrackingId;
            this.ProductId   = ProductId;
            this.UserId      = UserId;
            ConsumeDate      = DateTimeOffset.UtcNow;
            OrderId          = transaction.OrderId;
            OrderLineItemId  = transaction.OrderLineItemId;
            QuantityConsumed = transaction.QuantityConsumed;
            DbKey            = Guid.NewGuid().ToString();
        }
    }

    /// <summary>
    /// Tracks the current state of the clawback action items through the discovery and reconciliation process.
    /// </summary>
    public enum CompletedConsumeTransactionState
    {
        /// <summary>
        /// Transaction was granted to the user
        /// </summary>
        Granted = 0,

        /// <summary>
        /// Transaction was refunded to the user but has not yet been reconciled in our service
        /// </summary>
        Revoked,

        /// <summary>
        /// Transaction was refunded and we took appropriate action to reconcile the user's account
        /// </summary>
        Reconciled
    }
}
