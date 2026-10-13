# Chapter 7 — Reflection: Types, Members, and Architectural Rules

Architectural decisions often go unwritten as executable checks. A temporary `public Delete()` method on an entity can quietly become permanent when other code starts calling it—something functional tests won't catch because it doesn't break behavior, only contracts.

The answer is to turn verbal conventions into **executable assertions**. Rules like "all domain entities must be sealed" or "no public `Delete` methods" become part of your test suite, catching structural drift the moment it happens in CI.

## 7.1 Asserting on types

Start by checking a type's characteristics directly:

```csharp
typeof(Order).Should().BeSealed().And.NotBeAbstract();
typeof(Order).Should().BeAssignableTo<Entity>().And.NotBeAssignableTo<string>();
typeof(Order).Should().BeDecoratedWith<AuditedAttribute>();
typeof(Order).Should().NotBeDecoratedWith<ObsoleteAttribute>();
typeof(Order).Should().BeDecoratedWithOrInherit<AuditedAttribute>();
typeof(Order).Should().HaveAccessModifier(AccessModifier.Public);
```

The subtle difference: `BeDecoratedWith()` looks for the attribute on the current type (respecting normal attribute inheritance rules). `BeDecoratedWithOrInherit()` walks up the inheritance chain explicitly to find attributes—even those marked `Inherited = false` on a base class. That distinction matters when attributes opt out of inheritance.

Access modifiers are expressed via the explicit `AccessModifier` enum (`Public`, `Private`, `Protected`, `Internal`, `ProtectedInternal`, `PrivateProtected`), avoiding brittle string comparisons.

## 7.2 Members: methods, properties, constructors

To inspect a specific member, use `HaveMethod()`, `HaveProperty()`, or `HaveConstructor()`, then drill into details with `.Which`:

```csharp
var method = typeof(Order).Should().HaveMethod("Total").Which;
method.Should().Return<decimal>()
    .And.HaveParameter("currency", typeof(string))
    .And.BeVirtual()
    .And.NotBeStatic()
    .And.HaveAccessModifier(AccessModifier.Public);

var property = typeof(Order).Should().HaveProperty("Id").Which;
property.Should().Return<Guid>()
    .And.BeReadable()
    .And.NotBeWritable();

typeof(Order).Should().HaveConstructor(typeof(Guid), typeof(string));
typeof(Order).Should().HaveConstructor().Which.Should().BePublic();
```

This pins signatures precisely: parameter names and types, return types, static/virtual modifiers, and access levels. Any refactoring that changes the contract will break the test immediately instead of surfacing as a runtime surprise.

### Preventing forbidden members

The most powerful part of reflection assertions is **negation**:

```csharp
typeof(Order).Should().NotHaveMethod("Delete");
typeof(Order).Should().NotHaveProperty("PasswordHash");
```

This is exactly how you prevent "temporary" public members from becoming permanent by adoption. If a member shouldn't exist by contract, assert it doesn't.

Behavior note: if the type, `MethodInfo`, `PropertyInfo`, `ConstructorInfo`, or `Assembly` in question is `null`, positive assertions fail while negative assertions pass. In other words, absence means `NotHave*` passes safely—the correct behavior.

## 7.3 Assembly-level and architectural rules

Moving from a single type to the entire assembly lets you enforce design rules at the boundary level.

### Assembly assertions

```csharp
var assembly = typeof(Order).Assembly;

assembly.Should().DefineType("Store.Orders.Order");
assembly.Should().NotDefineType("Store.Orders.LegacyOrder");
assembly.Should().Reference(typeof(System.Text.Json.JsonSerializer).Assembly);
assembly.Should().NotReference(legacyAssembly);
```

Check for required types, forbid deprecated types, and assert assembly references as needed.

### Rules across all types

The most effective approach is to filter types, then assert a collective rule:

```csharp
AllTypes.From(typeof(Order).Assembly)
    .ThatDeriveFrom<Entity>()
    .Should().BeSealed();

AllTypes.From(typeof(Order).Assembly)
    .ThatAreInNamespace("Store.Orders")
    .Should().HaveCount(12);

AllTypes.From(typeof(Order).Assembly)
    .ThatImplement<IHandler>()
    .Should().HaveMethod("Handle");
```

Selectors compose naturally (`ThatDeriveFrom`, `ThatImplement`, `ThatAreInNamespace`, `ThatAreDecoratedWith`, etc.). The resulting assertions reuse the same vocabulary as single-type assertions (`BeSealed()`, `HaveCount()`, `HaveMethod()`, `HaveProperty()`, ...).

This gives you true **architectural tests** that run in CI like any other test. They only demand attention when they break—and that's exactly when they save you from silent structural drift.

### Member selectors: methods and properties

Types select members the same way assemblies select types. Start from a `Type`, filter, then assert—failures name the first offending member:

```csharp
typeof(OrderService).Methods().ThatArePublic().ThatReturnVoid().Should().HaveCount(2);

typeof(OrderService).Methods().ThatArePublic().Should().HaveAccessModifier(AccessModifier.Public);
typeof(OrderService).Properties().ThatAreReadOnly().Should().BeReadOnly();
typeof(OrderService).Methods().ThatAreDecoratedWith<MarkerAttribute>().Should().HaveAttribute<MarkerAttribute>();
```

Selections transition across kinds: method return types flow back into type selection via `ReturnTypes()`, and nested types are selected with `Types()` (compiler-generated types are excluded):

```csharp
typeof(OrderService).Methods().ThatAreAsync().ReturnTypes().Should().HaveCount(1);
typeof(OrderService).Types().Should().HaveCount(2);
```

Property filters cover accessibility (any accessor counts), static/virtual/abstract, read-only/write-only/writable, `OfType<T>`, attributes, and a single `ThatSatisfy()` predicate—there is deliberately no separate `Where()`. Filters are methods (not properties), matching this library's `TypeSelector` style rather than a mixed property/method style.

## 7.4 Recap and exercise

**Recap:**

- **Turn conventions into executable assertions.** Any architectural rule worth stating is worth asserting.
- **Go from type to member via `.Which`.** To pin signatures, access levels, modifiers, and mutability.
- **Use negation to protect contracts.** `NotHaveMethod()` prevents "temporary" members from becoming permanent.
- **Enforce rules at scale with `AllTypes.From()`.** Apply collective rules across your entire assembly in one go.

**Exercise:**

1. Require that **all domain entities** are sealed (`BeSealed()`).
2. Forbid any method named `Delete` from appearing in your domain namespace.
3. Assert the count of types in a specific namespace.
4. Intentionally add a violating member and watch the test catch it immediately.
