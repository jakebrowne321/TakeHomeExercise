# Pre-requisities

-   Git
-   Code editor (e.g Visual Studio / VSCode)
-   SQL Server 2014+
-   .NET 5

# Exercise Description

The following code test was created to evaluate your abilities as a .NET Developer. Some of the areas that you'll be evaluated in are:

-   Creativity
-   Proactivity
-   Code quality
-   Code organization
-   Tooling choices
-   Ability to follow instructions
-   Attention to details

We would like you to solve each task as they appear below, and avoid doing early optimizations or refactoring unless specified or if it blocks your solution for a specific problem. If any code is copied from the internet, make sure to add a comment with a reference to where you sourced it from.

Please treat this project and the tasks below as if they were part of, or the beginning of a bigger application that you would build and support. That means, please write the code as _you_ like to write it, not the way you think _we_ would like you to write it.

# Getting started

1. Initialise a new git repository at the root of the folder and perform an initial commit.
2. Restore the `Backend-TakeHomeExercise.bak` backup onto a SQL Server installation. There is a single table called `Listings` that you'll work with
3. Open the `TakeHomeExcercise.sln` solution and confirm you can build and run the API. You should see "TODO" being returned from the `/listings` route.
4. Happy coding. Good luck, and have fun! :)

# How to do the exercise

As you work on each task please prefix your commit(s) message(s) with the task reference using the following pattern:

> [Task-XX] My commit message

When you complete a task, as part of your commit and changes, please change the corresponding task status below from `Pending` to `Completed` and add any comments into the designated area at the bottom of each task, where you can explain what problems you found and how you solved them. You can also explain how you'd do something more complex and/or time consuming if you had the time for it, feel free to also add any other comments that are relevant to the task.

If you have made any changes to the DB (changed schema, added indexes, foreign keys, etc), please include what you've done in the comments section of the relevant section.

An example of the expected information in a comments section could be:

```
- Code currently does XXX. A possible enhancement later could be to do ZZZ
- Wasn't sure what was intended for scenario YYY here, so i made the call to do ZZZ
```

# Tasks

---

## `Task-01`: Create an API endpoint to return paged listings

_Status: `Completed`_

Create an API endpoint that returns paged listings from the sample DB, given the following (optional) filters:

-   Suburb (e.g "Southbank")
-   CategoryType (e.g "Rental")
-   StatusType (e.g "Current")
-   Skip (e.g 0)
-   Take (e.g 10)

Fields should be validated for invalid input, and the API should return an appropriate HTTP response when validation fails.

An example URL might be: `/listings?suburb=Southbank&categoryType=Rental&statusType=Current&take=10`

The returned JSON should look like:

```
{
  "items": [
    {
      "listingId": 4,
      "address": "53-55 Ellison St, Clifton Beach QLD 4879", // combination of address fields
      "categoryType": "Residential", // 1 = Residential, 2 = Rental, 3 = Land, 4 = Rural
      "statusType": "Current", // 1 = Current, 2 = Withdrawn, 3 = Sold, 4 = Leased, 5 = Off Market, 6 = Deleted
      "displayPrice": "Mid to High $800,000's",
      "title": "Buy me now!"
    }
  ],
  "total": 1212
}
```

```
Fairly straight forward api endpoint.
Dictionaries were needed for the mapping of categoryType and statusType as the parameters were
strings but the expected sql type was an integer.



The payload could also do with a 'next' & 'previous' link referring to the next and previous page the consumer of this endpoint
could reference for further data.

Also, I dont think postcode needs to be an integer in the database... do we need to do calculations on the postcode?, if not, I think it would be acceptable to be a string

```

---

## `Task-02`: Add caching by suburb

_Status: `Completed`_

A common use case for listings is returning current listings of a given type in a suburb. Because of this, we would like some basic caching adding to avoid the trip to the DB.

Please add this caching functionality, so that the following behavior occurs:

1. App loaded
2. `GET: /listings?suburb=Southbank&categoryType=Residential&take=10` -> cache MISS
3. `GET: /listings?suburb=Southbank&categoryType=Residential&take=10` -> cache HIT
4. `GET: /listings?suburb=Southbank&categoryType=Rental&take=10` -> cache MISS
5. `GET: /listings?suburb=Southbank&categoryType=Rental&take=10` -> cache HIT
6. `GET: /listings?suburb=Southbank&categoryType=Rental&take=5` -> cache HIT

```
Pretty simple task here.
Added response caching service in the startup.cs and configured the app to use responsecaching();
Added the caching attribute to the api endpoint which caches the suburb query string.
```

---

## `Task-03`: Add a new property shortPrice

_Status: `Completed`_

We would like a new prop added to the payload:

```
{
   ... // existing props
   "shortPrice": "$800k"
}
```

This is a short version of the `displayPrice`, to be used on things like the pin display.

Minimum scenarios to handle:

| displayPrice | shortPrice |
| ------------ | ---------- |
| $100         | "$100"     |
| $100,000     | "$100k"    |
| $1,500,000   | "$1.5m"    |
| For Sale     | ""         |

Bonus points for handling other scenarios (use your judgement as to what the value should be)

```
The data in the column "DisplayPrice" made it pretty difficult to convert to a short price.
Example would be that some rows contained text... which needed to be trimmed out before turning into a short price.
Where no price was given, I left a simple "Enquire for more info" message as there was no price to actually return in the short price
```

---

## `Task-04`: Tests

_Status: `Complete`_

It is expected that you write tests for important changes you made above on the previous tasks, if any test was relevant. As a last pass, please check any code, existing or new that would benefit from tests and write them. Explain below the benefits of the tests you wrote and why they are important.

```
The testing for this project would consist of testing the data passed into the endpoint, testing for data types, nulls etc.
This would make sure the api works as expected, and if the consumer of the api were to input wrong data, would not cause bugs.
I have attached some screenshots of my testing that I will add to the email I send to Ryan.

Eg, Expecting 422 when datatypes are incorrect
  Expecting 200 when data is correct

Also, this endpoint should require authentication before calling it... a JWT would work well


```

---
