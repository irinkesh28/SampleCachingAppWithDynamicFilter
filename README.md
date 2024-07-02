## Enable Caching Based on Provided Filters
I have added a logic in the EmployeeService class as below,
- Convert a Condition JSON to JSON string
- Used it as Cache key
- In case of any changes in Condition JSON payload it will create another cache of employee list
- If same Condition JSON found in the payload it will return list of employee form cache

## Implement Cache Invalidation on Code Change
I am not able to implement this but below is the way I tried,
Create a logic to calculate FileHashSet to compare with the original and changed file
Implement a clear cache validation in the middleware or else in CICD pipeline to execute automatically

## Propose and Implement Dynamic Filtering, Sorting, and Pagination
Implemented as below,
- Created a `Condition` Class with below properties,
```
    public class Condition
    {
        public ConditionType Type { get; set; } // Can be Single Or Group
        public LogicalOperator? LogicalOperator { get; set; } // Can be And/Or
        public Column? Column { get; set; }
        public ConditionOperator ConditionOperator { get; set; } // Can be any operator like Equals, Not Equals...
        public List<string> Values { get; set; } = []; // Can be 1 or couple in case of between filter
        public List<Condition> Conditions { get; set; } = []; // Can be multiple nested conditions
    }

```
- Created `LogicalExpressionBuilder` to implement all the operator and creates a logical where condition for each operator. This will return a 
   Query code as well as paramters with values to prevent injections.
- Created a `ConditionToCodeBuilder` class to build a logical query code. This will creates a Where condtions by combining with selected Logical operator ie. And or OR

>For the said assignment I implemented only few operators but this can be extended if we want to add more in future.
> This is built such a way by which we can accomodate any database dynamic query generation mechenism by creating another class by implementing `ILogicalExpressionBuilder`
>Added Sorting and Pagination as well. All this will be executed in database not in memory
Have added a Validator as well to validate payload.

## Demo
- Created a new Post endpoint for dynamic filter
- Not used the get one just to ease for better understading of payload.

```
http://localhost:5067/Employee
```
```
{
    "pageNo":1,
    "pageSize":100,
    "sortProperty":"Name",
    "ascendingSort":true,
    "condition":{
        "logicalOperator":0,
        "type": 0,
        "conditions":[
            {
                "type": 1,
                "column":{
                        "name":"id",
                        "dataType":0
                    },
                "conditionOperator":9,
                "values":["5"]
            },
            {
                "logicalOperator":1,
                "type": 0,
                "conditions":[
                    {
                        "type": 1,
                        "column":{
                            "name":"name",
                            "dataType":0
                        },
                        "conditionOperator":0,
                        "values":["Amit Kumar"]
                    },
                    {
                        "type":1,
                        "column":{
                            "name":"name1",
                            "dataType":0
                        },
                        "conditionOperator":0,
                        "values":["Neha Sharma"]
                    }
                ]
            }
        ]
    }
}
```
- Added a Condition JSON payload by which it can filter out those employees whose `name is Amit Kumar or Neha Shrma` And `whose Ids are Less than or Equal to 5`
