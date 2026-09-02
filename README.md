# Api-Design-Microservices

Solution for microservices apis

## NetNotificationApi -

=> MassTransit Workflow -

1. Always remove local queues after testing from queue_subscription table (ex: create_notification_queue_local has an "id" in queue table , remove the corresponding record from queue_subscription table).

2. Recommended Architecture ->

- Avoid:
  One giant consumer
  One giant switch(type)
  One giant queue

- Prefer:
  One shared contract
  ↓
  Multiple focused consumers
  ↓
  Independent queues

## NetCoreApi -

=> EventHandler Notifications Segment Workflow -

1.Changes done for event handlers

1. AcmeOrder
   --Create ( Added Email Template)
   --Update
   --Delete
   --Completed

2. AcmeProduct
   --Create ( Added Email Template)
   --Update
   --Delete
   --Completed
3. TodoItem
   --Create
   --Update
   --Delete
   --Completed

4. ToDoList
   --Create
   --Update
   --Delete
   --Completed

2) Need to assign the htmlTemplate and ToEmails properties proper values in NotificationDataMapping method,
   whenever updating or adding any event handler's notification part.
"# quincy-api" 
