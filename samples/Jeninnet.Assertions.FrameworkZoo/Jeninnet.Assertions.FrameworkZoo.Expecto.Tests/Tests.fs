module Tests

open System
open Expecto
open Jeninnet.Assertions
open Jeninnet.Assertions.FrameworkZoo.Core

let private newAccount () = BankAccount(100m)

let private bankAccountTests =
    testList "BankAccount" [
        testCase "An account starts with its opening balance" <| fun _ ->
            (newAccount()).Balance.Should().Be(100m).And.BeGreaterThan(0m) |> ignore

        testCase "Deposits increase the balance" <| fun _ ->
            let account = newAccount()
            account.Deposit(50m)
            account.Balance.Should().Be(150m, "a deposit adds its amount to the opening balance") |> ignore

        testCase "Withdrawals decrease the balance" <| fun _ ->
            let account = newAccount()
            account.Withdraw(40m)
            account.Balance.Should().Be(60m) |> ignore

        testCase "Withdrawing more than the balance fails" <| fun _ ->
            let withdraw = Action(fun () -> (newAccount()).Withdraw(1_000m))
            withdraw.Should().Throw<InvalidOperationException>().WithMessage("*insufficient*") |> ignore

        testCase "Independent checks are reported together" <| fun _ ->
            use _scope = new AssertionScope("the opening account")
            (newAccount()).Balance.Should().Be(100m) |> ignore
            (newAccount()).Balance.Should().BeGreaterThan(0m) |> ignore
            (newAccount()).Balance.Should().BeLessThan(1_000m) |> ignore

        testCase "The balance is compared structurally" <| fun _ ->
            (newAccount()).Should().BeEquivalentTo(BankAccount(100m)) |> ignore
    ]

let private fizzBuzzConversionTests =
    let fizzBuzz = FizzBuzz()

    testList "Conversions" [
        for (value, expected) in [ 1, "1"; 3, "Fizz"; 5, "Buzz"; 15, "FizzBuzz" ] ->
            testCase (sprintf "%d becomes %s" value expected) <| fun _ ->
                fizzBuzz.Convert(value).Should().Be(expected) |> ignore
    ]

let private fizzBuzzTests =
    let fizzBuzz = FizzBuzz()
    let custom = FizzBuzz(2, "Even", 7, "Lucky")

    testList "FizzBuzz" [
        fizzBuzzConversionTests

        testCase "A range contains one entry per number" <| fun _ ->
            fizzBuzz.ConvertRange(1, 15).Should().HaveCount(15).And.Contain("FizzBuzz").And.NotContain("16") |> ignore

        testCase "Custom words are configurable" <| fun _ ->
            custom.Convert(14).Should().Be("EvenLucky", "14 is divisible by both custom divisors") |> ignore
    ]

[<Tests>]
let all =
    testList "FrameworkZoo" [
        bankAccountTests
        fizzBuzzTests
    ]
