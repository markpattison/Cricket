define(["exports", "fable-core/umd/String"], function (exports, _String) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.formatRuns = formatRuns;
  exports.formatWickets = formatWickets;
  exports.formatWicketsLeft = formatWicketsLeft;

  function formatRuns(runs) {
    if (runs === 1) {
      return "1 run";
    } else if (runs > 1) {
      return (0, _String.fsFormat)("%i runs")(function (x) {
        return x;
      })(runs);
    } else {
      throw new Error("Invalid number of runs");
    }
  }

  function formatWickets(wickets) {
    if (wickets === 1) {
      return "1 wicket";
    } else if (wickets > 1) {
      return (0, _String.fsFormat)("%i wickets")(function (x) {
        return x;
      })(wickets);
    } else {
      throw new Error("Invalid number of wickets");
    }
  }

  function formatWicketsLeft(wickets) {
    return formatWickets(10 - wickets);
  }
});