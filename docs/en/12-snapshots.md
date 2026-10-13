# Chapter 12 — Snapshots: Frozen Outputs You Review On Purpose

Chapter 6 introduced `MatchSnapshot()` as a way to pin large outputs. This chapter is the operational companion: how the two snapshot files travel through your repository, how a failure reads, and the one deliberate switch that accepts new output in bulk — a switch that is never thrown by accident.

## 12.1 Two files, one decision

Every snapshot is a pair of files that live next to the test (or in the directory you pass):

- `<name>.verified.<ext>` — the **committed baseline**. This is the contract. It is reviewed, it is version-controlled, and it is what every future run is compared against.
- `<name>.received.<ext>` — the **candidate**. It is written only when the actual output does not match the baseline (missing baseline or a difference). It is a scratch artifact for you to inspect; it is never the source of truth.

`MatchSnapshot()` reduces to a single question: *is the newly produced output correct?* If yes, the baseline should move to it. If no, the code under test is wrong. Everything below exists to keep that decision explicit.

```csharp
invoiceText.Should().MatchSnapshot();                    // invoice.verified.txt next to the test
response.Should().MatchSnapshot("order-42", directory);  // explicit name and folder
order.Should().MatchSnapshot("order", directory, options =>
    options.Scrub(s => s.Replace(orderId, "<id>")));      // object → order.verified.json
```

## 12.2 The safe default

By default the assertion never accepts anything on its own:

1. **No `.verified` file exists** → a `.received` file is written and the test fails, with the received content shown inline. To accept, rename `.received` to `.verified`.
2. **Mismatch** → the `.received` file is written/updated, and the test fails with a unified diff. Review the diff; if the new output is correct, rename `.received` to `.verified`.
3. **Exact match** → any stale `.received` file is deleted and the test passes.

Because acceptance is a rename you perform, a snapshot can never silently drift toward whatever the code happens to produce. The default is deliberately conservative.

## 12.3 Accepting changes on purpose

Renaming one file is fine, but re-baselining dozens of snapshots by hand is tedious and error-prone. For that, set the update variable for the run:

```powershell
$env:JENINNET_SNAPSHOT_UPDATE = "1"
dotnet test
Remove-Item Env:JENINNET_SNAPSHOT_UPDATE
```

```bash
JENINNET_SNAPSHOT_UPDATE=1 dotnet test
```

When `JENINNET_SNAPSHOT_UPDATE` is exactly `1`, every `MatchSnapshot()` promotes its received text straight to the `.verified` file and passes; any stale `.received` file is removed. The rules that make this safe:

- **It is opt-in, per run.** The value must be exactly `1`. `0`, an empty string, or an unset variable all leave the strict default in place.
- **It is never implied.** No configuration file, no culture, and no CI default turns it on.
- **You review the diff.** The promoted files appear as normal changes in `git diff`; that review is the whole point.

After a bulk update, read the diff before committing. `JENINNET_SNAPSHOT_UPDATE` is a local re-baselining tool, not a switch for a green pipeline.

## 12.4 What the failure tells you

Both failure paths carry everything needed to decide, without opening files:

- **Missing baseline** shows the received content inline under a `Received:` heading, then points at the `.received` and `.verified` paths.
- **Mismatch** shows a unified diff (`--- expected` / `+++ actual`) so the change is visible at a glance.

You can append a reason with `because`, exactly like every other assertion, and the message tells the story of *why* the snapshot should hold.

## 12.5 Snapshots in CI

- **Never set `JENINNET_SNAPSHOT_UPDATE` in CI.** A pipeline accepts nothing; it only reports mismatches for a human to resolve.
- **Commit `*.verified.*` with `LF`.** Add `eol=lf` for these files in `.gitattributes` so a checkout on any OS compares byte-for-byte identically.
- **Never commit `*.received.*`.** Treat it like build output: ignore it and let it be recreated on the next failing run.
- **Scrub before you pin.** Mask volatile content (GUIDs, timestamps, request IDs) with `Scrub()` so the baseline captures the contract, not the noise.

## 12.6 Recap and exercise

**Recap:**

- **Two files, one decision.** `.verified` is the committed contract; `.received` is the candidate you inspect.
- **The default never auto-accepts.** Missing or different output fails; accepting is a rename you perform.
- **`JENINNET_SNAPSHOT_UPDATE=1`** promotes received to verified for the run and passes — opt-in, exact value, reviewed in `git diff`.
- **Failures carry their evidence.** The received content or a unified diff appears inline.
- **CI stays strict.** No update switch, `LF` baselines, no committed `.received`, volatile content scrubbed.

**Exercise:**

1. Pin a snapshot, then change the code so the output differs and run again — read the unified diff.
2. Re-baseline with `JENINNET_SNAPSHOT_UPDATE=1`, inspect the changed `.verified` file in `git diff`, then unset the variable.
3. Set `JENINNET_SNAPSHOT_UPDATE=0` and confirm the assertion fails again, proving only the exact value `1` opts in.
