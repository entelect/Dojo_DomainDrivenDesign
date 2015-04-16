##Domain Driven Design Dojo - Example
___
This Example shows an Anemic Domain and Rich Domain implementing the same set of Business Rules.

A rule change is then introduced with the changes reflected in each implementation.


___
**Business Rules Reflected in the 1.Anemic and 2.RichDomain Projects**

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

**Rule Change Reflected in the 3.Anemic and 4.RichDomain Projects**

Introduce a new **Sick Leave** type

* Use the same Approver Sequence as Annual Leave
* Can only be finalized if a sick note is provided

___

##Practical Example

**Gym Loyalty**

A Member on a Loyalty program qualifies for discounts on Gym Fees based on the Gym Club Type and the length of the Gym Membership. The business needs to calculate the refund amount monthly based on the following rules.

* There are three Club Types
    * Silver
    * Gold
    * Platinum
* If a member has been going to the gym for less than 12 Months, they get refunded a 	percentage.
    * Silver -> 40%
    * Gold -> 60%
    * Platinum -> 80%
* If a member has been going to the gym for more than 12 Months, they get refunded an amount 	per visit up to the total gym cost, but are required to visit the gym a minimum of 6 times 	per month to be eligible for a refund
    * Silver -> R30 per visit up to R500
    * Gold -> R50 up to R600
    * Platinum -> R75 up to R1000
* Assume you are provided with the monthly visits, club types and start date.
___
**Change 1**

Add a new **Blue** club type.
Cost R400
Gets Refunded R35 per visit up to R400 for a minimum of 6 visits per month from the start. (No initial % discount rule exists for the first month)

___
**Change 2**
Members can change club type.