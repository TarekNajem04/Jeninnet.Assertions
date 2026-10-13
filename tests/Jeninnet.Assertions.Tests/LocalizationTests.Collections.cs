//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class LocalizationTests {
    [TestMethod]
    public void Arabic_CollectionCounts() {
        using var _ = new ArabicScope();

        int[] values = [1, 2];
        var exact = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCount(3));
        Assert.AreEqual("المتوقع من values أن يحتوي 3 من العناصر، لكن وُجد 2.", exact.Message);

        int[]? missing = null;
        var exactNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveCount(0));
        Assert.AreEqual("المتوقع من missing أن يحتوي 0 من العناصر، لكن المجموعة null.", exactNull.Message);

        var more = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCountGreaterThan(5));
        Assert.AreEqual("المتوقع من values أن يحتوي أكثر من 5 من العناصر، لكن وُجد 2.", more.Message);

        var least = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCountGreaterThanOrEqualTo(5));
        Assert.AreEqual("المتوقع من values أن يحتوي على الأقل 5 من العناصر، لكن وُجد 2.", least.Message);

        var fewer = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCountLessThan(1));
        Assert.AreEqual("المتوقع من values أن يحتوي أقل من 1 من العناصر، لكن وُجد 2.", fewer.Message);

        var most = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCountLessThanOrEqualTo(1));
        Assert.AreEqual("المتوقع من values أن يحتوي على الأكثر 1 من العناصر، لكن وُجد 2.", most.Message);

        int[] trio = [1, 2, 3];
        var same = Assert.ThrowsExactly<AssertionFailedException>(() => trio.Should().HaveSameCount([1, 2]));
        Assert.AreEqual("المتوقع من trio أن يكون له نفس العدد (2) مثل المجموعة المتوقعة، لكن وُجد 3.", same.Message);

        var sameNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveSameCount([1]));
        Assert.AreEqual("المتوقع من missing أن يكون له نفس عدد المجموعة المتوقعة، لكن المجموعة null.", sameNull.Message);

        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeNull());
        Assert.AreEqual("المتوقع من values أن يكون null، لكن وُجد 2 من العناصر.", beNull.Message);

        var notBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNull());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد null.", notBeNull.Message);
    }

    [TestMethod]
    public void Arabic_CollectionEmptiness() {
        using var _ = new ArabicScope();

        int[] values = [1];
        var beEmpty = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeEmpty());
        Assert.AreEqual("المتوقع من values أن يكون فارغاً، لكن وُجد 1 من العناصر.", beEmpty.Message);

        int[]? missing = null;
        var beEmptyNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeEmpty());
        Assert.AreEqual("المتوقع من missing أن يكون فارغاً، لكن المجموعة null.", beEmptyNull.Message);

        int[] empty = [];
        var notBeEmpty = Assert.ThrowsExactly<AssertionFailedException>(() => empty.Should().NotBeEmpty());
        Assert.AreEqual("المتوقع من empty ألا يكون فارغاً، لكنه كان فارغاً.", notBeEmpty.Message);

        var notBeEmptyNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeEmpty());
        Assert.AreEqual("المتوقع من missing ألا يكون فارغاً، لكن المجموعة null.", notBeEmptyNull.Message);
    }

    [TestMethod]
    public void Arabic_CollectionContain() {
        using var _ = new ArabicScope();

        int[] values = [1, 2];
        var predicate = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().Contain(x => x > 10));
        Assert.AreEqual("المتوقع من values أن يحتوي عنصراً يطابق الشرط، لكن لم يُعثر على أي عنصر.", predicate.Message);

        int[]? missing = null;
        var predicateNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Contain(x => x > 10));
        Assert.AreEqual("المتوقع من missing أن يحتوي عنصراً يطابق الشرط، لكن المجموعة null.", predicateNull.Message);

        var item = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().Contain(9));
        Assert.AreEqual("المتوقع من values أن يحتوي '9'، لكنه لم يُعثر عليه.", item.Message);

        var itemNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Contain(9));
        Assert.AreEqual("المتوقع من missing أن يحتوي '9'، لكن المجموعة null.", itemNull.Message);

        var only = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().OnlyContain(x => x > 10));
        Assert.AreEqual("المتوقع من values أن يحتوي عناصر مطابقة فقط، لكن وُجد '1'.", only.Message);

        var onlyNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().OnlyContain(x => x > 10));
        Assert.AreEqual("المتوقع من missing أن يحتوي عناصر مطابقة فقط، لكن المجموعة null.", onlyNull.Message);

        var single = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().ContainSingle());
        Assert.AreEqual("المتوقع من values أن يحتوي عنصراً واحداً بالضبط، لكن وُجد 2.", single.Message);

        var singleNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainSingle());
        Assert.AreEqual("المتوقع من missing أن يحتوي عنصراً واحداً بالضبط، لكن المجموعة null.", singleNull.Message);

        var notItem = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotContain(2));
        Assert.AreEqual("المتوقع من values ألا يحتوي '2'، لكنه وُجد.", notItem.Message);

        var notItemNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotContain(2));
        Assert.AreEqual("المتوقع من missing ألا يحتوي '2'، لكن المجموعة null.", notItemNull.Message);

        var notPredicate = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotContain(x => x < 10));
        Assert.AreEqual("المتوقع من values ألا يحتوي عنصراً يطابق الشرط، لكن وُجد عنصر.", notPredicate.Message);

        var notPredicateNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotContain(x => x < 10));
        Assert.AreEqual("المتوقع من missing ألا يحتوي عنصراً يطابق الشرط، لكن المجموعة null.", notPredicateNull.Message);
    }

    [TestMethod]
    public void Arabic_CollectionEquivalence() {
        using var _ = new ArabicScope();

        int[]? missing = null;
        var nullEquivalent = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeEquivalentTo(1));
        Assert.AreEqual("المتوقع من missing أن يكون مكافئاً، لكن المجموعة null.", nullEquivalent.Message);

        int[] values = [1, 2, 3];
        var count = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeEquivalentTo(1, 2));
        Assert.AreEqual("المتوقع من values أن يحتوي 2 من العناصر، لكن وُجد 3.", count.Message);

        // S3878 waived: the params form cannot infer T alongside the config lambda.
        var order = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeEquivalentTo([3, 2, 1], static o => o.WithStrictOrdering()));
        Assert.AreEqual("المتوقع من values أن يكون مكافئاً بنفس الترتيب، لكن العنصر في الفهرس 0 كان '1' بدل '3'.", order.Message);

        int[] pair = [1, 1, 2];
        var cardinality = Assert.ThrowsExactly<AssertionFailedException>(() => pair.Should().BeEquivalentTo(1, 2, 3));
        Assert.AreEqual("المتوقع من pair أن يكون مكافئاً، لكن العنصر '1' وُجد 2 من المرات؛ والمتوقع 1.", cardinality.Message);
    }

    [TestMethod]
    public void Arabic_CollectionEqual() {
        using var _ = new ArabicScope();

        int[]? missing = null;
        var nullEqual = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Equal(1));
        Assert.AreEqual("المتوقع من missing أن يساوي ['1']، لكن المجموعة null.", nullEqual.Message);

        int[] values = [1, 2, 3];
        var count = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().Equal(1, 2));
        Assert.AreEqual("المتوقع من values أن يساوي ['1', '2']، لكن وُجد 3 من العناصر بدل 2.", count.Message);

        var order = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().Equal(3, 2, 1));
        Assert.AreEqual("المتوقع من values أن يساوي ['3', '2', '1']، لكن العنصر في الفهرس 0 كان '1' بدل '3'.", order.Message);
    }

    [TestMethod]
    public void Arabic_CollectionAffixes() {
        using var _ = new ArabicScope();

        int[]? missing = null;
        var startNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().StartWith(1, 9));
        Assert.AreEqual("المتوقع من missing أن يبدأ بـ ['1', '9']، لكن المجموعة null.", startNull.Message);

        int[] values = [1, 2, 3];
        var startShort = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().StartWith(1, 2, 3, 4));
        Assert.AreEqual("المتوقع من values أن يبدأ بـ ['1', '2', '3', '4']، لكن وُجد 3 من العناصر فقط.", startShort.Message);

        var startOrder = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().StartWith(1, 9));
        Assert.AreEqual("المتوقع من values أن يبدأ بـ ['1', '9']، لكن العنصر في الفهرس 1 كان '2'.", startOrder.Message);

        var endNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().EndWith(2, 3));
        Assert.AreEqual("المتوقع من missing أن ينتهي بـ ['2', '3']، لكن المجموعة null.", endNull.Message);

        var endShort = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().EndWith(9, 1, 2, 3));
        Assert.AreEqual("المتوقع من values أن ينتهي بـ ['9', '1', '2', '3']، لكن وُجد 3 من العناصر فقط.", endShort.Message);

        var endOrder = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().EndWith(1, 3));
        Assert.AreEqual("المتوقع من values أن ينتهي بـ ['1', '3']، لكن العنصر في الفهرس 1 كان '2'.", endOrder.Message);
    }

    [TestMethod]
    public void Arabic_CollectionOrder() {
        using var _ = new ArabicScope();

        int[]? missing = null;
        var sequenceNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainInOrder(1));
        Assert.AreEqual("المتوقع من missing أن يحتوي العناصر بالترتيب، لكن المجموعة null.", sequenceNull.Message);

        int[] values = [3, 1, 2];
        var sequenceMissing = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().ContainInOrder(1, 9));
        Assert.AreEqual("المتوقع من values أن يحتوي '9' بالترتيب، لكنه لم يُعثر عليه في التسلسل.", sequenceMissing.Message);

        var ascendingNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeInAscendingOrder());
        Assert.AreEqual("المتوقع من missing أن يكون بترتيب تصاعدي، لكن المجموعة null.", ascendingNull.Message);

        int[] pair = [2, 1];
        var ascending = Assert.ThrowsExactly<AssertionFailedException>(() => pair.Should().BeInAscendingOrder());
        Assert.AreEqual("المتوقع من pair أن يكون بترتيب تصاعدي، لكن وُجد '1' بعد '2' في الفهرس 1.", ascending.Message);

        var descendingNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeInDescendingOrder());
        Assert.AreEqual("المتوقع من missing أن يكون بترتيب تنازلي، لكن المجموعة null.", descendingNull.Message);

        int[] rising = [1, 2];
        var descending = Assert.ThrowsExactly<AssertionFailedException>(() => rising.Should().BeInDescendingOrder());
        Assert.AreEqual("المتوقع من rising أن يكون بترتيب تنازلي، لكن وُجد '2' بعد '1' في الفهرس 1.", descending.Message);
    }

    [TestMethod]
    public void Arabic_CollectionSets() {
        using var _ = new ArabicScope();

        int[]? missing = null;
        var subsetNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeSubsetOf([1, 3]));
        Assert.AreEqual("المتوقع من missing أن يكون مجموعة جزئية من ['1', '3']، لكن المجموعة null.", subsetNull.Message);

        int[] values = [1, 2];
        var outsider = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeSubsetOf([1, 3]));
        Assert.AreEqual("المتوقع من values أن يكون مجموعة جزئية من ['1', '3']، لكن وُجد '2' خارجها.", outsider.Message);

        var intersectNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().IntersectWith([3, 4]));
        Assert.AreEqual("المتوقع من missing أن يتقاطع مع ['3', '4']، لكن المجموعة null.", intersectNull.Message);

        var intersectEmpty = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().IntersectWith([3, 4]));
        Assert.AreEqual("المتوقع من values أن يتقاطع مع ['3', '4']، لكن لم تُعثر على عناصر مشتركة.", intersectEmpty.Message);

        var checkNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainSubset([9]));
        Assert.AreEqual("المتوقع من missing أن يحتوي المجموعة الجزئية، لكن المجموعة null.", checkNull.Message);

        var checkMissing = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().ContainSubset([9]));
        Assert.AreEqual("المتوقع من values أن يحتوي عنصر المجموعة الجزئية '9'، لكنه لم يُعثر عليه.", checkMissing.Message);
    }

    [TestMethod]
    public void Arabic_Dictionary() {
        using var _ = new ArabicScope();

        var scores = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var count = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().HaveCount(3));
        Assert.AreEqual("المتوقع من scores أن يحتوي 3 من العناصر، لكن وُجد 2.", count.Message);

        var empty = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().BeEmpty());
        Assert.AreEqual("المتوقع من scores أن يكون فارغاً، لكن وُجد 2 من العناصر.", empty.Message);

        var key = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().ContainKey("z"));
        Assert.AreEqual("المتوقع من scores أن يحتوي المفتاح 'z'، لكنه لم يُعثر عليه.", key.Message);

        var notKey = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotContainKey("a"));
        Assert.AreEqual("المتوقع من scores ألا يحتوي المفتاح 'a'، لكنه وُجد.", notKey.Message);

        var keys = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().ContainKeys("z"));
        Assert.AreEqual("المتوقع من scores أن يحتوي المفتاح 'z'، لكنه لم يُعثر عليه.", keys.Message);

        var notKeys = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotContainKeys("a", "b"));
        Assert.AreEqual("المتوقع من scores ألا يحتوي المفتاح 'a'، لكنه وُجد.", notKeys.Message);

        var value = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().ContainValue(9));
        Assert.AreEqual("المتوقع من scores أن يحتوي القيمة '9'، لكنه لم يُعثر عليها.", value.Message);

        var atKey = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().Contain("a", 2));
        Assert.AreEqual("المتوقع من scores أن تكون قيمته '2' عند المفتاح 'a'، لكن وُجد '1'.", atKey.Message);

        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().BeNull());
        Assert.AreEqual("المتوقع من scores أن يكون null، لكن وُجد 2 من العناصر.", beNull.Message);

        Dictionary<string, int>? missing = null;
        var haveCountNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveCount(3));
        Assert.AreEqual("المتوقع من missing أن يحتوي 3 من العناصر، لكن القاموس null.", haveCountNull.Message);

        var beEmptyNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeEmpty());
        Assert.AreEqual("المتوقع من missing أن يكون فارغاً، لكن القاموس null.", beEmptyNull.Message);

        var keyNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainKey("a"));
        Assert.AreEqual("المتوقع من missing أن يحتوي المفتاح 'a'، لكن القاموس null.", keyNull.Message);

        var valueNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainValue(1));
        Assert.AreEqual("المتوقع من missing أن يحتوي القيمة '1'، لكن القاموس null.", valueNull.Message);

        var keysNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainKeys("a"));
        Assert.AreEqual("المتوقع من missing أن يحتوي المفاتيح المعطاة، لكن القاموس null.", keysNull.Message);

        var notKeysNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotContainKeys("a"));
        Assert.AreEqual("المتوقع من missing ألا يحتوي المفاتيح المعطاة، لكن القاموس null.", notKeysNull.Message);

        var notKeyNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotContainKey("a"));
        Assert.AreEqual("المتوقع من missing ألا يحتوي المفتاح 'a'، لكن القاموس null.", notKeyNull.Message);

        var notBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNull());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد null.", notBeNull.Message);
    }

    [TestMethod]
    public void Arabic_CollectionSetRelationsExtended() {
        using var _ = new ArabicScope();

        int[] values = [1, 2];
        var superset = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeSupersetOf([1, 3]));
        Assert.AreEqual("المتوقع من values أن يكون مجموعة شاملة لـ['1', '3']، لكن '3' كان مفقودًا منها.", superset.Message);

        var properSubsetSame = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeProperSubsetOf([1, 2]));
        Assert.AreEqual("المتوقع من values أن يكون مجموعة جزئية فعلية من ['1', '2']، لكنهما يحتويان نفس العناصر.", properSubsetSame.Message);

        var properSupersetSame = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeProperSupersetOf([1, 2]));
        Assert.AreEqual("المتوقع من values أن يكون مجموعة شاملة فعلية لـ['1', '2']، لكنهما يحتويان نفس العناصر.", properSupersetSame.Message);

        int[]? missing = null;
        var supersetNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeSupersetOf([1]));
        Assert.AreEqual("المتوقع من missing أن يكون مجموعة شاملة لـ['1']، لكن المجموعة null.", supersetNull.Message);

        var properSubsetNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeProperSubsetOf([1]));
        Assert.AreEqual("المتوقع من missing أن يكون مجموعة جزئية فعلية من ['1']، لكن المجموعة null.", properSubsetNull.Message);

        var properSupersetNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeProperSupersetOf([1]));
        Assert.AreEqual("المتوقع من missing أن يكون مجموعة شاملة فعلية لـ['1']، لكن المجموعة null.", properSupersetNull.Message);
    }

    [TestMethod]
    public void Arabic_CollectionNullOrEmpty() {
        using var _ = new ArabicScope();

        int[] values = [1, 2];
        var beNullOrEmpty = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeNullOrEmpty());
        Assert.AreEqual("المتوقع من values أن يكون null أو فارغاً، لكن وُجد 2 من العناصر.", beNullOrEmpty.Message);

        int[]? missing = null;
        var notBeNullOrEmptyNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNullOrEmpty());
        Assert.AreEqual("المتوقع من missing ألا يكون null أو فارغاً، لكن المجموعة null.", notBeNullOrEmptyNull.Message);

        int[] empty = [];
        var notBeNullOrEmpty = Assert.ThrowsExactly<AssertionFailedException>(() => empty.Should().NotBeNullOrEmpty());
        Assert.AreEqual("المتوقع من empty ألا يكون null أو فارغاً، لكنه كان فارغاً.", notBeNullOrEmpty.Message);
    }

    [TestMethod]
    public void Arabic_DictionaryEqual() {
        using var _ = new ArabicScope();

        var scores = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var count = Assert.ThrowsExactly<AssertionFailedException>(
            () => scores.Should().Equal(new Dictionary<string, int> { ["a"] = 1 }));
        Assert.AreEqual("المتوقع من scores أن يساوي ['[a, 1]']، لكنه يحتوي 2 من العناصر بدل 1.", count.Message);

        var missingKey = Assert.ThrowsExactly<AssertionFailedException>(
            () => scores.Should().Equal(new Dictionary<string, int> { ["a"] = 1, ["z"] = 9 }));
        Assert.AreEqual("المتوقع من scores أن يساوي ['[a, 1]', '[z, 9]']، لكن المفتاح ''z'' لم يُعثر عليه.", missingKey.Message);

        var wrongValue = Assert.ThrowsExactly<AssertionFailedException>(
            () => scores.Should().Equal(new Dictionary<string, int> { ["a"] = 1, ["b"] = 9 }));
        Assert.AreEqual("المتوقع من scores أن يساوي ['[a, 1]', '[b, 9]']، لكن المفتاح ''b'' كان ''2'' بدل ''9''.", wrongValue.Message);

        IDictionary<string, int>? missingMap = null;
        var nullEqual = Assert.ThrowsExactly<AssertionFailedException>(() => missingMap.Should().Equal(scores));
        Assert.AreEqual("المتوقع من missingMap أن يساوي ['[a, 1]', '[b, 2]']، لكن القاموس null.", nullEqual.Message);
    }

    [TestMethod]
    public void Arabic_CollectionOrderingChains() {
        using var _ = new ArabicScope();

        var ascending = new[] { (Key: 1, Value: 2), (Key: 1, Value: 1) };
        var thenAsc = Assert.ThrowsExactly<AssertionFailedException>(() =>
            ascending.Should().BeInAscendingOrder(static x => x.Key).And.ThenBeInAscendingOrder(static x => x.Value));
        Assert.AreEqual("المتوقع من ascending أن يكون بعد ذلك بترتيب تصاعدي، لكن وُجد '(1, 1)' بعد '(1, 2)' في الفهرس 1.", thenAsc.Message);

        var descending = new[] { (Key: 1, Value: 1), (Key: 1, Value: 2) };
        var thenDesc = Assert.ThrowsExactly<AssertionFailedException>(() =>
            descending.Should().BeInAscendingOrder(static x => x.Key).And.ThenBeInDescendingOrder(static x => x.Value));
        Assert.AreEqual("المتوقع من descending أن يكون بعد ذلك بترتيب تنازلي، لكن وُجد '(1, 2)' بعد '(1, 1)' في الفهرس 1.", thenDesc.Message);
    }
}
