# F1 Optimization Playbook

This playbook defines the rules and assumptions Copilot must respect when proposing race strategy optimizations.

## Goal

Primary objective:

- Minimize total race time.

Secondary intent:

- Use tyres to the highest effective degradation without causing tyre blowouts.
- Optimize pit stop count and timing under race constraints.

Safety policy:

- Never intentionally allow corner crashes as a strategy.

## Units And Core Assumptions

- Units are meters and seconds.
- Race starts with car speed at 0.
- Constant acceleration and deceleration rates are provided in race spec.
- Maximum speed is provided in race spec.
- Corner speed is constant throughout each corner.
- Pit lane speed is used when exiting pit lane.
- Minimum crawl speed is provided in race spec.

## Segment Dynamics Rules

- Car can accelerate only on straights, up to target speed for that straight segment.
- Car can brake only on straights when preparing to enter a corner.
- No acceleration or deceleration is applied in limp mode.

## Car Modes

### Normal Mode

- Standard behavior using acceleration/deceleration and segment rules.

### Crawl Mode

Triggered when:

- Car enters a corner above allowed corner speed (corner crash).

Effects:

- Time penalty is applied immediately using `corner_crash_penalty_s`.
- Flat tyre damage penalty of `0.1` degradation is applied to current tyre set.
- Car travels at constant crawl speed until another straight is encountered.
- On that next straight, normal acceleration behavior may resume.

### Limp Mode

Triggered when:

- Fuel reaches 0, or
- Tyre reaches 0 (blowout).

Effects:

- Car travels at constant limp speed.
- No acceleration/deceleration while in limp mode.
- Limp mode continues until a pit stop is taken to refuel and/or change tyres.

## Pit Stop Rules

At pit stop, strategy may choose:

- Tyre change only.
- Refuel only.
- Both tyre change and refuel.

Pit stop has a time cost and must be included in total race time objective.

## Fuel And Speed Behavior

- Higher speeds consume more fuel.
- Acceleration consumes more fuel than steady-state running.

## Weather Effects

Weather affects:

- Acceleration
- Deceleration
- Tyre wear

Weather states:

- Dry
- Cold
- Light Rain
- Heavy Rain

## Global Tyre Degradation Constants

Use these constants as provided:

- `K_STRAIGHT = 0.0000166`
- `K_BRAKING = 0.0398`
- `K_CORNER = 0.000265`

Note:

- Final composition formula will be supplied later in code. Treat these as authoritative constants now.

## Tyre Compounds

Supported compounds:

- Soft
- Medium
- Hard
- Intermediate
- Wet

## Tyre Properties Table

| Property                       | Soft | Medium | Hard | Intermediate |  Wet |
| ------------------------------ | ---: | -----: | ---: | -----------: | ---: |
| Base Friction Coefficient      |  1.8 |    1.7 |  1.6 |          1.2 |  1.1 |
| Dry Multiplier                 | 1.18 |   1.08 | 0.98 |         0.90 | 0.72 |
| Cold Multiplier                | 1.00 |   0.97 | 0.92 |         0.96 | 0.88 |
| Light Rain Multiplier          | 0.92 |   0.88 | 0.82 |         1.08 | 1.02 |
| Heavy Rain Multiplier          | 0.80 |   0.74 | 0.68 |         1.02 | 1.20 |
| Dry Rate Of Degradation        | 0.14 |   0.10 | 0.07 |         0.11 | 0.16 |
| Cold Rate Of Degradation       | 0.11 |   0.08 | 0.06 |         0.09 | 0.12 |
| Light Rain Rate Of Degradation | 0.12 |   0.09 | 0.07 |         0.08 | 0.09 |
| Heavy Rain Rate Of Degradation | 0.13 |   0.10 | 0.08 |         0.09 | 0.05 |

## Penalty Rules

1. Corner overspeed causes:

- Increased tyre wear
- Time penalty (`corner_crash_penalty_s`)
- Transition into crawl mode

2. Fuel depletion or tyre blowout causes:

- Transition into limp mode

## Optimization Decision Variables

When generating strategy, optimize over:

- Target speed per straight segment.
- Braking initiation zone on straights before corners.
- Pit stop timing.
- Pit stop action type (tyres/refuel/both).
- Tyre compound selection per stint.

## Hard Constraints Checklist

Any proposed strategy must satisfy all:

- No intentional corner crash strategies.
- Respect max speed.
- Accelerate only on straights.
- Brake only on straights before corners.
- Maintain constant speed in corners.
- In limp mode, no acceleration or deceleration.
- Pit actions required to recover from limp mode.
- Include pit time and penalties in total race time.

## Copilot Prompt Template: Strategy Synthesis

Use this prompt when asking Copilot to generate a strategy:

```text
Use F1_OPTIMIZATION_PLAYBOOK.md as the governing rulebook.
Generate an end-to-end race strategy that minimizes total race time.

Inputs:
- Track segments (straight/corner with lengths and corner speed limits)
- Initial fuel, tank capacity, fuel burn model inputs
- Tyre start compound and available sets
- Weather timeline by lap/segment
- Pit lane delta time and pit service times
- Acceleration/deceleration/max speed/crawl speed/limp speed constants

Output required:
1) Per-segment target speed plan
2) Braking zones before each corner
3) Pit stop plan (lap, action, tyres, refuel amount)
4) Estimated total time including penalties
5) Constraint compliance report (explicitly confirm each hard constraint)

Never output a plan that intentionally relies on corner crashes.
If constraints are infeasible, explain why and return the closest feasible recovery plan.
```

## Copilot Prompt Template: Strategy Sanity Check

Use this prompt when validating a proposed strategy:

```text
Validate this strategy against F1_OPTIMIZATION_PLAYBOOK.md.

Checks:
- Any corner overspeed events?
- Any illegal acceleration/deceleration segments?
- Any limp-mode violations?
- Any missed pit recovery after fuel/tyre reaches zero?
- Total time accounting includes pit and crash penalties?
- Tyre and weather properties applied correctly?

Return:
1) PASS/FAIL
2) Violations list with location (lap/segment)
3) Time impact of each violation
4) Minimal set of changes to make strategy feasible
```

## Implementation Note

When formula code is provided later:

- Lock the degradation composition equation.
- Keep this playbook updated so planning and code remain aligned.
