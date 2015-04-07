##Domain Driven Design Dojo - Example
___
This Example shows an Anaemic Domain and Rich Domain implementing the same set of Business Rules.

A rule change is then introduced with the changes reflected in each implementation.
___
**Business Rules Reflected in the 1.Anaemic and 2.RichDomain Projects**

* As an **Employee** I want to
    * Capture Leave
    * Capture Different Types of Leave
        * Annual
        * Study
    * Escalate leave to a different approver. Approver sequence for each leave type is as follows
        * Annual - Team Lead -> Account Manager -> General Manager
        * Study - Account Manager -> General Manager
    * Change My Leave Type. This returns it to a pending status
* As an **Approver** I want to
    * Approve Leave that is assigned To me
* As a **Manager** I want to finalize leave that has been approved

___

**Rule Change Reflected in the 3.Anaemic and 4.RichDomain Projects**

Intoduce a new **Sick Leave** type

* Use the same Approver Sequence as Annual Leave
* Can only be finalized if a sick note is provided
