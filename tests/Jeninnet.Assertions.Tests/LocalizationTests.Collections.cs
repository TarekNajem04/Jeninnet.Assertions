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
        var exact = Throws<AssertionFailedException>(() => values.Should().HaveCount(3));
        exact.Message.Should().Be("المتوقع من values أن يحتوي 3 من العناصر، لكن وُجد 2.");

        int[]? missing = null;
        var exactNull = Throws<AssertionFailedException>(() => missing.Should().HaveCount(0));
        exactNull.Message.Should().Be("المتوقع من missing أن يحتوي 0 من العناصر، لكن المجموعة null.");

        var more = Throws<AssertionFailedException>(() => values.Should().HaveCountGreaterThan(5));
        more.Message.Should().Be("المتوقع من values أن يحتوي أكثر من 5 من العناصر، لكن وُجد 2.");

        var least = Throws<AssertionFailedException>(() => values.Should().HaveCountGreaterThanOrEqualTo(5));
        least.Message.Should().Be("المتوقع من values أن يحتوي على الأقل 5 من العناصر، لكن وُجد 2.");

        var fewer = Throws<AssertionFailedException>(() => values.Should().HaveCountLessThan(1));
        fewer.Message.Should().Be("المتوقع من values أن يحتوي أقل من 1 من العناصر، لكن وُجد 2.");

        var most = Throws<AssertionFailedException>(() => values.Should().HaveCountLessThanOrEqualTo(1));
        most.Message.Should().Be("المتوقع من values أن يحتوي على الأكثر 1 من العناصر، لكن وُجد 2.");

        int[] trio = [1, 2, 3];
        var same = Throws<AssertionFailedException>(() => trio.Should().HaveSameCount([1, 2]));
        same.Message.Should().Be("المتوقع من trio أن يكون له نفس العدد (2) مثل المجموعة المتوقعة، لكن وُجد 3.");

        var sameNull = Throws<AssertionFailedException>(() => missing.Should().HaveSameCount([1]));
        sameNull.Message.Should().Be("المتوقع من missing أن يكون له نفس عدد المجموعة المتوقعة، لكن المجموعة null.");

        var beNull = Throws<AssertionFailedException>(() => values.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من values أن يكون null، لكن وُجد 2 من العناصر.");

        var notBeNull = Throws<AssertionFailedException>(() => missing.Should().NotBeNull());
        notBeNull.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد null.");
    }

    [TestMethod]
    public void Arabic_CollectionEmptiness() {
        using var _ = new ArabicScope();

        int[] values = [1];
        var beEmpty = Throws<AssertionFailedException>(() => values.Should().BeEmpty());
        beEmpty.Message.Should().Be("المتوقع من values أن يكون فارغاً، لكن وُجد 1 من العناصر.");

        int[]? missing = null;
        var beEmptyNull = Throws<AssertionFailedException>(() => missing.Should().BeEmpty());
        beEmptyNull.Message.Should().Be("المتوقع من missing أن يكون فارغاً، لكن المجموعة null.");

        int[] empty = [];
        var notBeEmpty = Throws<AssertionFailedException>(() => empty.Should().NotBeEmpty());
        notBeEmpty.Message.Should().Be("المتوقع من empty ألا يكون فارغاً، لكنه كان فارغاً.");

        var notBeEmptyNull = Throws<AssertionFailedException>(() => missing.Should().NotBeEmpty());
        notBeEmptyNull.Message.Should().Be("المتوقع من missing ألا يكون فارغاً، لكن المجموعة null.");
    }

    [TestMethod]
    public void Arabic_CollectionContain() {
        using var _ = new ArabicScope();

        int[] values = [1, 2];
        var predicate = Throws<AssertionFailedException>(() => values.Should().Contain(x => x > 10));
        predicate.Message.Should().Be("المتوقع من values أن يحتوي عنصراً يطابق الشرط، لكن لم يُعثر على أي عنصر.");

        int[]? missing = null;
        var predicateNull = Throws<AssertionFailedException>(() => missing.Should().Contain(x => x > 10));
        predicateNull.Message.Should().Be("المتوقع من missing أن يحتوي عنصراً يطابق الشرط، لكن المجموعة null.");

        var item = Throws<AssertionFailedException>(() => values.Should().Contain(9));
        item.Message.Should().Be("المتوقع من values أن يحتوي '9'، لكنه لم يُعثر عليه.");

        var itemNull = Throws<AssertionFailedException>(() => missing.Should().Contain(9));
        itemNull.Message.Should().Be("المتوقع من missing أن يحتوي '9'، لكن المجموعة null.");

        var only = Throws<AssertionFailedException>(() => values.Should().OnlyContain(x => x > 10));
        only.Message.Should().Be("المتوقع من values أن يحتوي عناصر مطابقة فقط، لكن وُجد '1'.");

        var onlyNull = Throws<AssertionFailedException>(() => missing.Should().OnlyContain(x => x > 10));
        onlyNull.Message.Should().Be("المتوقع من missing أن يحتوي عناصر مطابقة فقط، لكن المجموعة null.");

        var single = Throws<AssertionFailedException>(() => values.Should().ContainSingle());
        single.Message.Should().Be("المتوقع من values أن يحتوي عنصراً واحداً بالضبط، لكن وُجد 2.");

        var singleNull = Throws<AssertionFailedException>(() => missing.Should().ContainSingle());
        singleNull.Message.Should().Be("المتوقع من missing أن يحتوي عنصراً واحداً بالضبط، لكن المجموعة null.");

        var notItem = Throws<AssertionFailedException>(() => values.Should().NotContain(2));
        notItem.Message.Should().Be("المتوقع من values ألا يحتوي '2'، لكنه وُجد.");

        var notItemNull = Throws<AssertionFailedException>(() => missing.Should().NotContain(2));
        notItemNull.Message.Should().Be("المتوقع من missing ألا يحتوي '2'، لكن المجموعة null.");

        var notPredicate = Throws<AssertionFailedException>(() => values.Should().NotContain(x => x < 10));
        notPredicate.Message.Should().Be("المتوقع من values ألا يحتوي عنصراً يطابق الشرط، لكن وُجد عنصر.");

        var notPredicateNull = Throws<AssertionFailedException>(() => missing.Should().NotContain(x => x < 10));
        notPredicateNull.Message.Should().Be("المتوقع من missing ألا يحتوي عنصراً يطابق الشرط، لكن المجموعة null.");
    }

    [TestMethod]
    public void Arabic_CollectionEquivalence() {
        using var _ = new ArabicScope();

        int[]? missing = null;
        var nullEquivalent = Throws<AssertionFailedException>(() => missing.Should().BeEquivalentTo(1));
        nullEquivalent.Message.Should().Be("المتوقع من missing أن يكون مكافئاً، لكن المجموعة null.");

        int[] values = [1, 2, 3];
        var count = Throws<AssertionFailedException>(() => values.Should().BeEquivalentTo(1, 2));
        count.Message.Should().Be("المتوقع من values أن يحتوي 2 من العناصر، لكن وُجد 3.");

        // S3878 waived: the params form cannot infer T alongside the config lambda.
        var order = Throws<AssertionFailedException>(() => values.Should().BeEquivalentTo([3, 2, 1], static o => o.WithStrictOrdering()));
        order.Message.Should().Be("المتوقع من values أن يكون مكافئاً بنفس الترتيب، لكن العنصر في الفهرس 0 كان '1' بدل '3'.");

        int[] pair = [1, 1, 2];
        var cardinality = Throws<AssertionFailedException>(() => pair.Should().BeEquivalentTo(1, 2, 3));
        cardinality.Message.Should().Be("المتوقع من pair أن يكون مكافئاً، لكن العنصر '1' وُجد 2 من المرات؛ والمتوقع 1.");
    }

    [TestMethod]
    public void Arabic_CollectionEqual() {
        using var _ = new ArabicScope();

        int[]? missing = null;
        var nullEqual = Throws<AssertionFailedException>(() => missing.Should().Equal(1));
        nullEqual.Message.Should().Be("المتوقع من missing أن يساوي ['1']، لكن المجموعة null.");

        int[] values = [1, 2, 3];
        var count = Throws<AssertionFailedException>(() => values.Should().Equal(1, 2));
        count.Message.Should().Be("المتوقع من values أن يساوي ['1', '2']، لكن وُجد 3 من العناصر بدل 2.");

        var order = Throws<AssertionFailedException>(() => values.Should().Equal(3, 2, 1));
        order.Message.Should().Be("المتوقع من values أن يساوي ['3', '2', '1']، لكن العنصر في الفهرس 0 كان '1' بدل '3'.");
    }

    [TestMethod]
    public void Arabic_CollectionAffixes() {
        using var _ = new ArabicScope();

        int[]? missing = null;
        var startNull = Throws<AssertionFailedException>(() => missing.Should().StartWith(1, 9));
        startNull.Message.Should().Be("المتوقع من missing أن يبدأ بـ ['1', '9']، لكن المجموعة null.");

        int[] values = [1, 2, 3];
        var startShort = Throws<AssertionFailedException>(() => values.Should().StartWith(1, 2, 3, 4));
        startShort.Message.Should().Be("المتوقع من values أن يبدأ بـ ['1', '2', '3', '4']، لكن وُجد 3 من العناصر فقط.");

        var startOrder = Throws<AssertionFailedException>(() => values.Should().StartWith(1, 9));
        startOrder.Message.Should().Be("المتوقع من values أن يبدأ بـ ['1', '9']، لكن العنصر في الفهرس 1 كان '2'.");

        var endNull = Throws<AssertionFailedException>(() => missing.Should().EndWith(2, 3));
        endNull.Message.Should().Be("المتوقع من missing أن ينتهي بـ ['2', '3']، لكن المجموعة null.");

        var endShort = Throws<AssertionFailedException>(() => values.Should().EndWith(9, 1, 2, 3));
        endShort.Message.Should().Be("المتوقع من values أن ينتهي بـ ['9', '1', '2', '3']، لكن وُجد 3 من العناصر فقط.");

        var endOrder = Throws<AssertionFailedException>(() => values.Should().EndWith(1, 3));
        endOrder.Message.Should().Be("المتوقع من values أن ينتهي بـ ['1', '3']، لكن العنصر في الفهرس 1 كان '2'.");
    }

    [TestMethod]
    public void Arabic_CollectionOrder() {
        using var _ = new ArabicScope();

        int[]? missing = null;
        var sequenceNull = Throws<AssertionFailedException>(() => missing.Should().ContainInOrder(1));
        sequenceNull.Message.Should().Be("المتوقع من missing أن يحتوي العناصر بالترتيب، لكن المجموعة null.");

        int[] values = [3, 1, 2];
        var sequenceMissing = Throws<AssertionFailedException>(() => values.Should().ContainInOrder(1, 9));
        sequenceMissing.Message.Should().Be("المتوقع من values أن يحتوي '9' بالترتيب، لكنه لم يُعثر عليه في التسلسل.");

        var ascendingNull = Throws<AssertionFailedException>(() => missing.Should().BeInAscendingOrder());
        ascendingNull.Message.Should().Be("المتوقع من missing أن يكون بترتيب تصاعدي، لكن المجموعة null.");

        int[] pair = [2, 1];
        var ascending = Throws<AssertionFailedException>(() => pair.Should().BeInAscendingOrder());
        ascending.Message.Should().Be("المتوقع من pair أن يكون بترتيب تصاعدي، لكن وُجد '1' بعد '2' في الفهرس 1.");

        var descendingNull = Throws<AssertionFailedException>(() => missing.Should().BeInDescendingOrder());
        descendingNull.Message.Should().Be("المتوقع من missing أن يكون بترتيب تنازلي، لكن المجموعة null.");

        int[] rising = [1, 2];
        var descending = Throws<AssertionFailedException>(() => rising.Should().BeInDescendingOrder());
        descending.Message.Should().Be("المتوقع من rising أن يكون بترتيب تنازلي، لكن وُجد '2' بعد '1' في الفهرس 1.");
    }

    [TestMethod]
    public void Arabic_CollectionSets() {
        using var _ = new ArabicScope();

        int[]? missing = null;
        var subsetNull = Throws<AssertionFailedException>(() => missing.Should().BeSubsetOf([1, 3]));
        subsetNull.Message.Should().Be("المتوقع من missing أن يكون مجموعة جزئية من ['1', '3']، لكن المجموعة null.");

        int[] values = [1, 2];
        var outsider = Throws<AssertionFailedException>(() => values.Should().BeSubsetOf([1, 3]));
        outsider.Message.Should().Be("المتوقع من values أن يكون مجموعة جزئية من ['1', '3']، لكن وُجد '2' خارجها.");

        var intersectNull = Throws<AssertionFailedException>(() => missing.Should().IntersectWith([3, 4]));
        intersectNull.Message.Should().Be("المتوقع من missing أن يتقاطع مع ['3', '4']، لكن المجموعة null.");

        var intersectEmpty = Throws<AssertionFailedException>(() => values.Should().IntersectWith([3, 4]));
        intersectEmpty.Message.Should().Be("المتوقع من values أن يتقاطع مع ['3', '4']، لكن لم تُعثر على عناصر مشتركة.");

        var checkNull = Throws<AssertionFailedException>(() => missing.Should().ContainSubset([9]));
        checkNull.Message.Should().Be("المتوقع من missing أن يحتوي المجموعة الجزئية، لكن المجموعة null.");

        var checkMissing = Throws<AssertionFailedException>(() => values.Should().ContainSubset([9]));
        checkMissing.Message.Should().Be("المتوقع من values أن يحتوي عنصر المجموعة الجزئية '9'، لكنه لم يُعثر عليه.");
    }

    [TestMethod]
    public void Arabic_CollectionApproximately() {
        using var _ = new ArabicScope();

        int[] actual = [1, 2, 3];
        var order = Throws<AssertionFailedException>(() => actual.Should().EqualApproximately([1, 2, 4], 0));
        order.Message.Should().Be("المتوقع من actual أن يساوي تقريباً ['1', '2', '4']، لكن العنصر في الفهرس 2 كان '3' بدل '4' بدقة '0'.");

        var count = Throws<AssertionFailedException>(() => actual.Should().EqualApproximately([1, 2], 0));
        count.Message.Should().Be("المتوقع من actual أن يساوي تقريباً ['1', '2']، لكن وُجد 3 من العناصر بدل 2.");

        int[]? missing = null;
        var nullCollection = Throws<AssertionFailedException>(() => missing.Should().EqualApproximately([1], 0));
        nullCollection.Message.Should().Be("المتوقع من missing أن يساوي تقريباً ['1']، لكن المجموعة null.");

        var notEqual = Throws<AssertionFailedException>(() => actual.Should().NotEqualApproximately([1, 2, 3], 0));
        notEqual.Message.Should().Be("المتوقع من actual ألا يساوي تقريباً ['1', '2', '3']، لكنهما متساويان تقريباً بدقة '0'.");

        var notEqualNull = Throws<AssertionFailedException>(() => missing.Should().NotEqualApproximately([1], 0));
        notEqualNull.Message.Should().Be("المتوقع من missing ألا يساوي تقريباً ['1']، لكن المجموعة null.");
    }

    [TestMethod]
    public void Arabic_Dictionary() {
        using var _ = new ArabicScope();

        var scores = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var count = Throws<AssertionFailedException>(() => scores.Should().HaveCount(3));
        count.Message.Should().Be("المتوقع من scores أن يحتوي 3 من العناصر، لكن وُجد 2.");

        var empty = Throws<AssertionFailedException>(() => scores.Should().BeEmpty());
        empty.Message.Should().Be("المتوقع من scores أن يكون فارغاً، لكن وُجد 2 من العناصر.");

        var key = Throws<AssertionFailedException>(() => scores.Should().ContainKey("z"));
        key.Message.Should().Be("المتوقع من scores أن يحتوي المفتاح 'z'، لكنه لم يُعثر عليه.");

        var notKey = Throws<AssertionFailedException>(() => scores.Should().NotContainKey("a"));
        notKey.Message.Should().Be("المتوقع من scores ألا يحتوي المفتاح 'a'، لكنه وُجد.");

        var keys = Throws<AssertionFailedException>(() => scores.Should().ContainKeys("z"));
        keys.Message.Should().Be("المتوقع من scores أن يحتوي المفتاح 'z'، لكنه لم يُعثر عليه.");

        var notKeys = Throws<AssertionFailedException>(() => scores.Should().NotContainKeys("a", "b"));
        notKeys.Message.Should().Be("المتوقع من scores ألا يحتوي المفتاح 'a'، لكنه وُجد.");

        var value = Throws<AssertionFailedException>(() => scores.Should().ContainValue(9));
        value.Message.Should().Be("المتوقع من scores أن يحتوي القيمة '9'، لكنه لم يُعثر عليها.");

        var atKey = Throws<AssertionFailedException>(() => scores.Should().Contain("a", 2));
        atKey.Message.Should().Be("المتوقع من scores أن تكون قيمته '2' عند المفتاح 'a'، لكن وُجد '1'.");

        var moreThan = Throws<AssertionFailedException>(() => scores.Should().HaveCountGreaterThan(2));
        moreThan.Message.Should().Be("المتوقع من scores أن يحتوي أكثر من 2 من العناصر، لكن وُجد 2.");

        var atLeast = Throws<AssertionFailedException>(() => scores.Should().HaveCountGreaterThanOrEqualTo(3));
        atLeast.Message.Should().Be("المتوقع من scores أن يحتوي على الأقل 3 من العناصر، لكن وُجد 2.");

        var fewerThan = Throws<AssertionFailedException>(() => scores.Should().HaveCountLessThan(2));
        fewerThan.Message.Should().Be("المتوقع من scores أن يحتوي أقل من 2 من العناصر، لكن وُجد 2.");

        var atMost = Throws<AssertionFailedException>(() => scores.Should().HaveCountLessThanOrEqualTo(1));
        atMost.Message.Should().Be("المتوقع من scores أن يحتوي على الأكثر 1 من العناصر، لكن وُجد 2.");

        var single = Throws<AssertionFailedException>(() => scores.Should().ContainSingle());
        single.Message.Should().Be("المتوقع من scores أن يحتوي عنصراً واحداً بالضبط، لكن وُجد 2.");

        IReadOnlyDictionary<string, int> readOnly = scores;
        var readOnlyCount = Throws<AssertionFailedException>(() => readOnly.Should().HaveCount(3));
        readOnlyCount.Message.Should().Be("المتوقع من readOnly أن يحتوي 3 من العناصر، لكن وُجد 2.");

        var beNull = Throws<AssertionFailedException>(() => scores.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من scores أن يكون null، لكن وُجد 2 من العناصر.");

        Dictionary<string, int>? missing = null;
        var haveCountNull = Throws<AssertionFailedException>(() => missing.Should().HaveCount(3));
        haveCountNull.Message.Should().Be("المتوقع من missing أن يحتوي 3 من العناصر، لكن القاموس null.");

        var beEmptyNull = Throws<AssertionFailedException>(() => missing.Should().BeEmpty());
        beEmptyNull.Message.Should().Be("المتوقع من missing أن يكون فارغاً، لكن القاموس null.");

        var keyNull = Throws<AssertionFailedException>(() => missing.Should().ContainKey("a"));
        keyNull.Message.Should().Be("المتوقع من missing أن يحتوي المفتاح 'a'، لكن القاموس null.");

        var valueNull = Throws<AssertionFailedException>(() => missing.Should().ContainValue(1));
        valueNull.Message.Should().Be("المتوقع من missing أن يحتوي القيمة '1'، لكن القاموس null.");

        var keysNull = Throws<AssertionFailedException>(() => missing.Should().ContainKeys("a"));
        keysNull.Message.Should().Be("المتوقع من missing أن يحتوي المفاتيح المعطاة، لكن القاموس null.");

        var notKeysNull = Throws<AssertionFailedException>(() => missing.Should().NotContainKeys("a"));
        notKeysNull.Message.Should().Be("المتوقع من missing ألا يحتوي المفاتيح المعطاة، لكن القاموس null.");

        var notKeyNull = Throws<AssertionFailedException>(() => missing.Should().NotContainKey("a"));
        notKeyNull.Message.Should().Be("المتوقع من missing ألا يحتوي المفتاح 'a'، لكن القاموس null.");

        var notBeNull = Throws<AssertionFailedException>(() => missing.Should().NotBeNull());
        notBeNull.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد null.");

        var moreThanNull = Throws<AssertionFailedException>(() => missing.Should().HaveCountGreaterThan(2));
        moreThanNull.Message.Should().Be("المتوقع من missing أن يحتوي أكثر من 2 من العناصر، لكن القاموس null.");

        var atLeastNull = Throws<AssertionFailedException>(() => missing.Should().HaveCountGreaterThanOrEqualTo(2));
        atLeastNull.Message.Should().Be("المتوقع من missing أن يحتوي على الأقل 2 من العناصر، لكن القاموس null.");

        var fewerThanNull = Throws<AssertionFailedException>(() => missing.Should().HaveCountLessThan(2));
        fewerThanNull.Message.Should().Be("المتوقع من missing أن يحتوي أقل من 2 من العناصر، لكن القاموس null.");

        var atMostNull = Throws<AssertionFailedException>(() => missing.Should().HaveCountLessThanOrEqualTo(2));
        atMostNull.Message.Should().Be("المتوقع من missing أن يحتوي على الأكثر 2 من العناصر، لكن القاموس null.");

        var singleNull = Throws<AssertionFailedException>(() => missing.Should().ContainSingle());
        singleNull.Message.Should().Be("المتوقع من missing أن يحتوي عنصراً واحداً بالضبط، لكن القاموس null.");
    }

    [TestMethod]
    public void Arabic_CollectionSetRelationsExtended() {
        using var _ = new ArabicScope();

        int[] values = [1, 2];
        var superset = Throws<AssertionFailedException>(() => values.Should().BeSupersetOf([1, 3]));
        superset.Message.Should().Be("المتوقع من values أن يكون مجموعة شاملة لـ['1', '3']، لكن '3' كان مفقودًا منها.");

        var properSubsetSame = Throws<AssertionFailedException>(() => values.Should().BeProperSubsetOf([1, 2]));
        properSubsetSame.Message.Should().Be("المتوقع من values أن يكون مجموعة جزئية فعلية من ['1', '2']، لكنهما يحتويان نفس العناصر.");

        var properSupersetSame = Throws<AssertionFailedException>(() => values.Should().BeProperSupersetOf([1, 2]));
        properSupersetSame.Message.Should().Be("المتوقع من values أن يكون مجموعة شاملة فعلية لـ['1', '2']، لكنهما يحتويان نفس العناصر.");

        int[]? missing = null;
        var supersetNull = Throws<AssertionFailedException>(() => missing.Should().BeSupersetOf([1]));
        supersetNull.Message.Should().Be("المتوقع من missing أن يكون مجموعة شاملة لـ['1']، لكن المجموعة null.");

        var properSubsetNull = Throws<AssertionFailedException>(() => missing.Should().BeProperSubsetOf([1]));
        properSubsetNull.Message.Should().Be("المتوقع من missing أن يكون مجموعة جزئية فعلية من ['1']، لكن المجموعة null.");

        var properSupersetNull = Throws<AssertionFailedException>(() => missing.Should().BeProperSupersetOf([1]));
        properSupersetNull.Message.Should().Be("المتوقع من missing أن يكون مجموعة شاملة فعلية لـ['1']، لكن المجموعة null.");
    }

    [TestMethod]
    public void Arabic_CollectionNullOrEmpty() {
        using var _ = new ArabicScope();

        int[] values = [1, 2];
        var beNullOrEmpty = Throws<AssertionFailedException>(() => values.Should().BeNullOrEmpty());
        beNullOrEmpty.Message.Should().Be("المتوقع من values أن يكون null أو فارغاً، لكن وُجد 2 من العناصر.");

        int[]? missing = null;
        var notBeNullOrEmptyNull = Throws<AssertionFailedException>(() => missing.Should().NotBeNullOrEmpty());
        notBeNullOrEmptyNull.Message.Should().Be("المتوقع من missing ألا يكون null أو فارغاً، لكن المجموعة null.");

        int[] empty = [];
        var notBeNullOrEmpty = Throws<AssertionFailedException>(() => empty.Should().NotBeNullOrEmpty());
        notBeNullOrEmpty.Message.Should().Be("المتوقع من empty ألا يكون null أو فارغاً، لكنه كان فارغاً.");
    }

    [TestMethod]
    public void Arabic_DictionaryEqual() {
        using var _ = new ArabicScope();

        var scores = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var count = Throws<AssertionFailedException>(
            () => scores.Should().Equal(new Dictionary<string, int> { ["a"] = 1 }));
        count.Message.Should().Be("المتوقع من scores أن يساوي ['[a, 1]']، لكنه يحتوي 2 من العناصر بدل 1.");

        var missingKey = Throws<AssertionFailedException>(
            () => scores.Should().Equal(new Dictionary<string, int> { ["a"] = 1, ["z"] = 9 }));
        missingKey.Message.Should().Be("المتوقع من scores أن يساوي ['[a, 1]', '[z, 9]']، لكن المفتاح ''z'' لم يُعثر عليه.");

        var wrongValue = Throws<AssertionFailedException>(
            () => scores.Should().Equal(new Dictionary<string, int> { ["a"] = 1, ["b"] = 9 }));
        wrongValue.Message.Should().Be("المتوقع من scores أن يساوي ['[a, 1]', '[b, 9]']، لكن المفتاح ''b'' كان ''2'' بدل ''9''.");

        IDictionary<string, int>? missingMap = null;
        var nullEqual = Throws<AssertionFailedException>(() => missingMap.Should().Equal(scores));
        nullEqual.Message.Should().Be("المتوقع من missingMap أن يساوي ['[a, 1]', '[b, 2]']، لكن القاموس null.");
    }

    [TestMethod]
    public void Arabic_CollectionOrderingChains() {
        using var _ = new ArabicScope();

        var ascending = new[] { (Key: 1, Value: 2), (Key: 1, Value: 1) };
        var thenAsc = Throws<AssertionFailedException>(() =>
            ascending.Should().BeInAscendingOrder(static x => x.Key).And.ThenBeInAscendingOrder(static x => x.Value));
        thenAsc.Message.Should().Be("المتوقع من ascending أن يكون بعد ذلك بترتيب تصاعدي، لكن وُجد '(1, 1)' بعد '(1, 2)' في الفهرس 1.");

        var descending = new[] { (Key: 1, Value: 1), (Key: 1, Value: 2) };
        var thenDesc = Throws<AssertionFailedException>(() =>
            descending.Should().BeInAscendingOrder(static x => x.Key).And.ThenBeInDescendingOrder(static x => x.Value));
        thenDesc.Message.Should().Be("المتوقع من descending أن يكون بعد ذلك بترتيب تنازلي، لكن وُجد '(1, 2)' بعد '(1, 1)' في الفهرس 1.");
    }
}
