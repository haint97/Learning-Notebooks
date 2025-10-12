# API (Application Programming Interface)
An API (Application Programming Interface) is a set of rules and protocols that allows different software applications to communicate with each other. APIs define the methods and data formats that applications can use to request and exchange information.

## Introduction
### API Design
API design is carefully planning, preparing, and developing programming interfaces (APIs) to expose data and the system’s functionality to consumers. APIs enable system-to-system communication and are essential for digital organizations because they add new capabilities to their products, operations, partnership strategies, and more. An effective API design is one that has satisfactory answers to the following queries of a developer:

    - Why is the API being developed?
    - What would be the outcome regarding the impact and output of the system?
    - How will the API be designed to meet the requirements?
    - What will be the structure of our resources?
    - How will we document our resources?

#### Types of APIs

| API type     | Authentication type                                              | Potential users                                            | Examples                                 |
|--------------|------------------------------------------------------------------|------------------------------------------------------------|------------------------------------------|
| Public APIs  | Publicly accessible with API keys                                 | B2C (business-to-consumer)                                 | Google Maps, Weather APIs                |
| Private APIs | No authentication                                                 | B2B (business-to-business), B2C, B2E (business-to-employee)| Educative APIs for creating courses      |
| Partner APIs | Authorized access with access tokens/license                      | B2B, B2C                                                  | Amazon APIs for partners                 |
| Composite APIs | Depends on the connected API's authentication                   | B2B, B2C, B2E                                             | Payment APIs (Stripe, PayPal)            |


#### API design lifecycle

```mermaid
flowchart TB
  START([START]) --> Req[Requirements]
  Req -->|API design lifecycle| Design((Design))
  Req -->|API lifecycle| Dev((Develop))
  Design --> Dev
  Dev --> Deploy((Deploy))
  Deploy --> Publish((Publish))
  Publish --> Use((Use))
  Design -->|Prototype| Use
  Use --> Analyze((Analyze))
  Analyze --> Update((Update))
  Update --> Design

  %% Blue = API design lifecycle
  linkStyle 1 stroke:#1f6feb,stroke-width:2px
  linkStyle 3 stroke:#1f6feb,stroke-width:2px
  linkStyle 7 stroke:#1f6feb,stroke-width:2px
  linkStyle 8 stroke:#1f6feb,stroke-width:2px
  linkStyle 9 stroke:#1f6feb,stroke-width:2px
  linkStyle 10 stroke:#1f6feb,stroke-width:2px

  %% Black = API lifecycle
  linkStyle 0 stroke:#000,stroke-width:1px
  linkStyle 2 stroke:#000,stroke-width:2px
  linkStyle 4 stroke:#000,stroke-width:2px
  linkStyle 5 stroke:#000,stroke-width:2px
  linkStyle 6 stroke:#000,stroke-width:2px
```

#### API design considerations
- **Identify user types**
    - Partners, customers, external developers
    - Determines access levels, authentication, and architectural style (REST, gRPC, etc.)

- **Developer problems & business value**
    - Address needs based on business relationship and critical requirements
    - Value addition: improved revenue, task speed, cost efficiency

- **API responses & error handling**
    - Clearly define success and error responses for endpoints
    - Implement robust exception and error handling

- **Real-life use cases & testability**
    - Apply practical scenarios to validate effectiveness
    - Analyze performance under various use cases

- **Scalability**
    - Design for future growth and increased customer demand

- **Documentation**
    - Provide comprehensive guides for integration, behaviors, structures, and parameters

#### Characteristics of a good API design

| Characteristics                                   | Explanation                                                                                                                                                                                                                           |
|---------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Separation between API specification and its implementation | • Includes separation between the specification and its implementation, that is, the behavior with the internal structural details <br> • Clean designs allow iterative improvements and changes to the API implementation             |
| Concurrency                                       | • Amount of API calls that can be active simultaneously in a specified period <br> • Useful in ensuring that computing resources are available for all users                                                                          |
| Dynamic rate-limiting                             | • Strategy to limit access to API within a timeframe <br> • Avoids overwhelming the API with an onslaught of requests                                                                          |
| Security                                          | • Well-defined security mechanisms for authentication and authorization protocols that will define who can access the API and what parts of the API they can access                             |
| Error warnings and handling                       | • Allows error handling effectively to prevent frustration on the consumer end <br> • Reduces debugging efforts for developers                                                                 |
| Architectural styles of an API                    | • Possible to follow different architectural styles according to its requirements                                                                                                              |
| Minimal but comprehensive and cohesive            | • API should be as terse as possible but fulfill its goals                                                                                                                                    |
| Stateless or state-bearing                        | • API functions can be stateless and/or maintain their state, but idempotency (operations that yield the same result when they are performed multiple times [Source: Wikipedia]) is a desired feature                                |
| User adoption                                     | • APIs that have good adoption often have a devoted user community that helps improve the API over many iterations                                                                             |
| Fault tolerance                                   | • Failures are inevitable, but a well-designed API can be made fault-tolerant by using mechanisms that ensure the continued operation of the API, even if some components malfunction           |
| Performance measurement                           | • There should be appropriate provisions for collecting monitoring data and early warning systems                                                                                              |
