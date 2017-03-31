.. _dependency_injection:

Dependency injection
================================

Dependency injection is a core concept of Protogame; it allows games to be built
in an extremely flexible and scalable way, without locking down early design
or architectural decisions.

Dependency injection does have a cost in game development, often it takes a little
longer to wire up new systems as you'll need to register them in the dependency
injection kernel at startup.  However, dependency injection significantly pays off
in the long term as it almost completely eliminates the need for large refactors;
any system which is to be replaced can be done so gracefully by providing a shim
for older interfaces.

The usage of dependency injection is what allows your games to scale in Protogame
from the prototyping stage to full development, without throwing away prototype
code.  You can gradually migrate away from your prototype code one system at a
time, without being forced to do a "big bang" approach to refactors.

Quick summary
-------------------

Dependency injection involves two main tasks:

* Registering an interface to an implementation in the kernel
* Adding the dependencies of your implementation to it's constructor

We'll go into more details on each of these concepts in the next sections, but
a very rough summary of what both of these look like in code follows.  To
register an interface in the kernel, you'd add the following binding code either
to the program startup, or inside a dependency injection module:

.. literalinclude:: snippet/di_summary_binding.cs
    :language: csharp
    
then in the class where you want to use the functionality of the interface or
implementation, you would inject it into your class constructor like so:

.. literalinclude:: snippet/di_summary_inject.cs
    :language: csharp

Now you can see how this allows systems within your game to be changed out
extremely easily; when you need to change the implementation of a system
you only need to change it in one place: at the point of binding.

For example, you could implement your player movement calculation code in
an ``IPlayerMovement`` interface and ``DefaultPlayerMovement`` implementation.  In
C# you would make ``DefaultPlayerMovement`` implement ``IPlayerMovement``, and
write the appropriate calculation code inside the implementation.  Later on
in development if you want to start trialling a new movement system, you
don't need to replace or directly change the existing implementation; you
can create a new implementation like ``NewPlayerMovement`` (preferably more
sensibly named), and change the binding to bind to ``NewPlayerMovement``
instead.  **Now everywhere in your game that uses player movement will start
using the new code, without any refactoring.**

In depth: Binding services
----------------------------

TODO

In depth: Injecting services
----------------------------

TODO

Creating game entities with factories
---------------------------------------

TODO