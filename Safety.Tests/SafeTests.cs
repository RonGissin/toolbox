using FluentAssertions;
using TestStack.BDDfy;
using ToolBox.Safety;

namespace Safety.Tests;

[TestClass]
public class SafeTests
{
    [TestInitialize]
    public void Initialize()
    {
        _argument = null;
        _enumerableArgument = null;
        _enumerableResult = null;
        _result = null;
        _ex = null;
        _guidArgument = Guid.Empty;
        _guidResult = Guid.Empty;
        _intArgument = 0;
        _intResult = 0;
        _upperBound = 0;
        _lowerBound = 0;
        _condition = false;
        _type = null;
    }

    [TestMethod]
    public void ThrowIfNull_throws_if_null()
    {
        this.Given(_ => A_null_argument())
            .When(_ => Calling_ThrowIfNull())
            .Then(_ => Exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfNull_does_not_throw_if_not_null()
    {
        this.Given(_ => A_non_null_non_empty_argument())
            .When(_ => Calling_ThrowIfNull())
            .Then(_ => No_exception_is_thrown())
            .And(_ => Result_is_same_as_argument())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfNullOrEmpty_throws_if_null()
    {
        this.Given(_ => A_null_argument())
            .When(_ => Calling_ThrowIfNullOrEmpty())
            .Then(_ => Exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfNullOrEmpty_throws_if_empty()
    {
        this.Given(_ => An_empty_string_argument())
            .When(_ => Calling_ThrowIfNullOrEmpty())
            .Then(_ => Exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfNullOrEmpty_does_not_throw_if_non_empty()
    {
        this.Given(_ => A_non_null_non_empty_argument())
            .When(_ => Calling_ThrowIfNullOrEmpty())
            .Then(_ => No_exception_is_thrown())
            .And(_ => Result_is_same_as_argument())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfNullOrEmptyEnumerable_throws_if_null()
    {
        this.Given(_ => A_null_enumerable_argument())
            .When(_ => Calling_ThrowIfNullOrEmptyEnumerable())
            .Then(_ => Exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfNullOrEmptyEnumerable_throws_if_empty_enumerable()
    {
        this.Given(_ => An_empty_enumerable())
            .When(_ => Calling_ThrowIfNullOrEmptyEnumerable())
            .Then(_ => Exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfNullOrEmptyEnumerable_does_not_throw_if_non_empty_enumerable()
    {
        this.Given(_ => A_non_null_non_empty_enumerable())
            .When(_ => Calling_ThrowIfNullOrEmptyEnumerable())
            .Then(_ => No_exception_is_thrown())
            .And(_ => Enumerable_result_is_the_same_as_argument())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfEmptyGuid_throws_if_empty_guid()
    {
        this.Given(_ => An_empty_guid_argument())
            .When(_ => Calling_ThrowIfEmptyGuid())
            .Then(_ => Exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfEmptyGuid_does_not_throw_if_non_empty_guid()
    {
        this.Given(_ => A_non_empty_guid_argument())
            .When(_ => Calling_ThrowIfEmptyGuid())
            .Then(_ => No_exception_is_thrown())
            .And(_ => Guid_result_is_the_same_as_argument())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfAboveUpperBound_throws_if_above_upper_bound()
    {
        this.Given(_ => An_int_argument(11))
            .And(_ => An_upper_bound(10))
            .When(_ => Calling_ThrowIfAboveUpperBound())
            .Then(_ => Exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfAboveUpperBound_does_not_throw_if_equal_to_upper_bound()
    {
        this.Given(_ => An_int_argument(10))
            .And(_ => An_upper_bound(10))
            .When(_ => Calling_ThrowIfAboveUpperBound())
            .Then(_ => No_exception_is_thrown())
            .And(_ => Int_result_is_the_same_as_argument())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfAboveUpperBound_does_not_throw_if_below_upper_bound()
    {
        this.Given(_ => An_int_argument(9))
            .And(_ => An_upper_bound(10))
            .When(_ => Calling_ThrowIfAboveUpperBound())
            .Then(_ => No_exception_is_thrown())
            .And(_ => Int_result_is_the_same_as_argument())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfBelowLowerBound_throws_if_below_lower_bound()
    {
        this.Given(_ => An_int_argument(9))
            .And(_ => A_lower_bound(10))
            .When(_ => Calling_ThrowIfBelowLowerBound())
            .Then(_ => Exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfBelowLowerBound_does_not_throw_if_equal_to_lower_bound()
    {
        this.Given(_ => An_int_argument(10))
            .And(_ => A_lower_bound(10))
            .When(_ => Calling_ThrowIfBelowLowerBound())
            .Then(_ => No_exception_is_thrown())
            .And(_ => Int_result_is_the_same_as_argument())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfBelowLowerBound_does_not_throw_if_above_lower_bound()
    {
        this.Given(_ => An_int_argument(11))
            .And(_ => A_lower_bound(10))
            .When(_ => Calling_ThrowIfBelowLowerBound())
            .Then(_ => No_exception_is_thrown())
            .And(_ => Int_result_is_the_same_as_argument())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfNotBetween_throws_if_below_lower_bound()
    {
        this.Given(_ => An_int_argument(9))
            .And(_ => A_lower_bound(10))
            .And(_ => An_upper_bound(20))
            .When(_ => Calling_ThrowIfNotBetween())
            .Then(_ => Exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfNotBetween_throws_if_above_upper_bound()
    {
        this.Given(_ => An_int_argument(21))
            .And(_ => A_lower_bound(10))
            .And(_ => An_upper_bound(20))
            .When(_ => Calling_ThrowIfNotBetween())
            .Then(_ => Exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIfNotBetween_does_not_throw_if_between_bounds()
    {
        this.Given(_ => An_int_argument(15))
            .And(_ => A_lower_bound(10))
            .And(_ => An_upper_bound(20))
            .When(_ => Calling_ThrowIfNotBetween())
            .Then(_ => No_exception_is_thrown())
            .And(_ => Int_result_is_the_same_as_argument())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIf_throws_if_true_condition()
    {
        this.Given(_ => A_condition_with_value(true))
            .When(_ => Calling_ThrowIf())
            .Then(_ => Exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void ThrowIf_does_not_throw_if_false_condition()
    {
        this.Given(_ => A_condition_with_value(false))
            .When(_ => Calling_ThrowIf())
            .Then(_ => No_exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void AssertIsAssignableFrom_throws_if_not_assignable_from()
    {
        this.Given(_ => A_type(typeof(string)))
            .When(_ => Calling_AssertIsAssignableFrom<SomeClass>())
            .Then(_ => Exception_is_thrown())
            .BDDfy();
    }

    [TestMethod]
    public void AssertIsAssignableFrom_does_not_throw_if_assignable_from()
    {
        this.Given(_ => A_type(typeof(string)))
            .When(_ => Calling_AssertIsAssignableFrom<object>())
            .Then(_ => No_exception_is_thrown())
            .BDDfy();
    }

    [Given]
    private void A_null_argument()
    {
        _argument = null;
    }

    [Given]
    private void A_non_null_non_empty_argument()
    {
        _argument = "not null";
    }

    [Given]
    private void An_empty_guid_argument()
    {
        _guidArgument = Guid.Empty;
    }

    [Given]
    private void A_non_empty_guid_argument()
    {
        _guidArgument = Guid.NewGuid();
    }

    [Given]
    private void An_empty_string_argument()
    {
        _argument = string.Empty;
    }

    [Given]
    private void A_null_enumerable_argument()
    {
        _enumerableArgument = null;
    }

    [Given]
    private void An_empty_enumerable()
    {
        _enumerableArgument = Enumerable.Empty<string>();
    }

    [Given]
    private void A_non_null_non_empty_enumerable()
    {
        _enumerableArgument = ["not null"];
    }

    [Given]
    private void An_int_argument(int argument)
    {
        _intArgument = argument;
    }

    [Given]
    private void An_upper_bound(int upperBound)
    {
        _upperBound = upperBound;
    }

    [Given]
    private void A_lower_bound(int lowerBound)
    {
        _lowerBound = lowerBound;
    }

    [Given]
    private void A_condition_with_value(bool value)
    {
        _condition = value;
    }

    [Given]
    private void A_type(Type type)
    {
        _type = type;
    }

    [When]
    private void Calling_ThrowIfNull()
    {
        try
        {
            _result = Safe.ThrowIfNull(_argument);
        }
        catch (ArgumentNullException ex)
        {
            _ex = ex;
        }
    }

    [When]
    private void Calling_ThrowIfNullOrEmpty()
    {
        try
        {
            _result = Safe.ThrowIfNullOrEmpty(_argument);
        }
        catch (ArgumentException ex)
        {
            _ex = ex;
        }
    }

    [When]
    private void Calling_ThrowIfNullOrEmptyEnumerable()
    {
        try
        {
            _enumerableResult = Safe.ThrowIfNullOrEmptyEnumerable(_enumerableArgument);
        }
        catch (ArgumentException ex)
        {
            _ex = ex;
        }
    }

    [When]
    private void Calling_ThrowIfEmptyGuid()
    {
        try
        {
            _guidResult = Safe.ThrowIfEmptyGuid(_guidArgument);
        }
        catch (ArgumentException ex)
        {
            _ex = ex;
        }
    }

    [When]
    private void Calling_ThrowIfAboveUpperBound()
    {
        try
        {
            _intResult = Safe.ThrowIfAboveUpperBound(_intArgument, _upperBound);
        }
        catch (ArgumentException ex)
        {
            _ex = ex;
        }
    }

    [When]
    private void Calling_ThrowIfBelowLowerBound()
    {
        try
        {
            _intResult = Safe.ThrowIfBelowLowerBound(_intArgument, _lowerBound);
        }
        catch (ArgumentException ex)
        {
            _ex = ex;
        }
    }

    [When]
    private void Calling_ThrowIfNotBetween()
    {
        try
        {
            _intResult = Safe.ThrowIfNotBetween(_intArgument, _lowerBound, _upperBound);
        }
        catch (ArgumentException ex)
        {
            _ex = ex;
        }
    }

    [When]
    private void Calling_ThrowIf()
    {
        try
        {
            Safe.ThrowIf(_condition, "condition is true");
        }
        catch (ArgumentException ex)
        {
            _ex = ex;
        }
    }

    [When]
    private void Calling_AssertIsAssignableFrom<T>()
        where T : class
    {
        try
        {
            Safe.AssertIsAssignableFrom<T>(_type);
        }
        catch (ArgumentException ex)
        {
            _ex = ex;
        }
    }

    [Then]
    private void Exception_is_thrown()
    {
        _ex.Should().NotBeNull();
    }

    [Then]
    private void No_exception_is_thrown()
    {
        _ex.Should().BeNull();
    }

    [Then]
    private void Result_is_same_as_argument()
    {
        _result.Should().Be(_argument);
    }

    [Then]
    private void Enumerable_result_is_the_same_as_argument()
    {
        _enumerableResult.Should().BeEquivalentTo(_enumerableArgument);
    }

    [Then]
    private void Guid_result_is_the_same_as_argument()
    {
        _guidResult.Should().Be(_guidArgument);
    }

    [Then]
    private void Int_result_is_the_same_as_argument()
    {
        _intResult.Should().Be(_intArgument);
    }

    private IEnumerable<string> _enumerableArgument;
    private IEnumerable<string> _enumerableResult;
    private string _argument;
    private Guid _guidArgument;
    private int _intArgument;
    private int _intResult;
    private int _upperBound;
    private int _lowerBound;
    private Guid _guidResult;
    private string _result;
    private bool _condition;
    private Type _type;
    private Exception _ex;

    private class SomeClass
    {
    }
}