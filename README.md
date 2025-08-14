# OscarGKTest

## Steps

While working on this assignment, I split the first couple of commits out in a way that I hope helps illustrate my initial thought process to tackling this task

### 1. Add `Speaker.cs` code file, resolve compiler errors ([8a30987])

My first move was to drop in the `Speaker.cs` file into the project and resolve all compiler errors. This was done mostly by replacing the missing models and enums that the file was referencing. This commit barely changes the `Speaker.cs` file. This allow me to get the code working, while giving me time to get familier with the intended behaviour of the file before I make any changes to it.

### 2. Split `Speaker.cs` into two seperate files: a database entity and a request ([c974258])

This commit, I began refactoring. I decided to address what I believed to be the biggest issue with `Speaker.cs` - which is that all of the properties were public, and, when registering, the user could pass in the same values as paramaters. Then, inside the method, the code was not actually using any of the paramaters other than `repository`.

To address this issue, I split `Speaker.cs` into two files. One, kept the `Speaker.cs` name and moved into the `Models` directory. This file now represents my database entity only - it contains no business logic.

The second file is called `RegisterSpeakerRequest.cs` and contains all the logic and validation for registering a new speaker. It accepts a `RegisterSpeakerRequest` object and saves a `Speaker` object to the database if validation passes.

This change:

- Creates a clear seperation of concerns between the database entities and business logic
- Removes any ambiguity around which values the "Register" code should be using, as it can now no longer access both the `Speaker` properties and the method paramaters.
- Simplifies unit testing - We now just unit test `RegisterSpeakerRequest.cs`.

While doing this, I rewrote the "Register" code to remove most of the nesting, and renamed most variables.

### 3. Small refactorings, add unit tests

The subsequent commits continued to tweak and tidy the "Register" code, as well as adding Unit tests.

## Concessions

I've made a number of concessions in the code, that I've not actioned, either due to time or because they are not relevent enough to the task I have been set.

- I saw that in the provided code file, the `Register` method takes in some paramaters as strings, but on the `Speaker` model, they are `List<>`s, or `WebBrowser` objects. What I take from this, is that the system that this code lives within captures those fields from the User as strings, and then there will be some process somewhere to convert those strings to more representitive models. It would be reasonable to say that I've made things easier for myself by making this assumption. It could be that `Speaker.cs` should be responsible for this mapping, for example, taking a `strBrowser` and creating a `WebBrowser` object using that string. Or taking in the `csvSess` string and converting to a `List<Sessions>`. My code does not do this, instead assuming that another part of the system should be responsible for this conversion before passing the request to `RegisterSpeakerRequest`
- My approach here mimics how I would typically work, but I have not spent the time to setup either MediatR, Dependency Injection or Entity framework. I've made some methods static, and kept the `IRepository` paramater to get around this.

## Further Improvements

If given more time to work on this project, I would:

- Improve the unit tests. I've left specific comments in the `RegisterSpeakerRequestHandlerTests.cs` file as to what I would do to improve some tests.
- Implement Dependency Injection, setup `RegisterSpeakerRequest` as a proper request using MediatR. Setup more of the database side, including adding Entity Framework. Remove `IReposity` as a paramater of `RegisterSpeaker`. Some of this would be overkill for a simple console application, but I believe in a production web application, this is more likely to be what we would see.
- Perhaps look at converting the string values of `strBrowser`, `csvCertifications` and `csvSess` into their correct forms using resonable assumptions about the contents of those strings.
- Potentially use FluentValidtion to simplify some validation checks.

[8a30987]: https://github.com/OscarGKTest/GKTest/commit/8a3098791b4028ea59242b153f17c236aba091b6
[c974258]: https://github.com/OscarGKTest/GKTest/commit/c9742582add027927e230339d27566fe1c330fb2
