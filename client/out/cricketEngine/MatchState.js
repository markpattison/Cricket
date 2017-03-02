define(["exports", "fable-core/umd/Symbol", "./Innings", "fable-core/umd/Util", "./Types"], function (exports, _Symbol2, _Innings, _Util, _Types) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.MatchStateModule = exports.MatchUpdate = exports.SummaryMatchState = exports.MatchState = undefined;

  var _Symbol3 = _interopRequireDefault(_Symbol2);

  function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
      default: obj
    };
  }

  function _defineProperty(obj, key, value) {
    if (key in obj) {
      Object.defineProperty(obj, key, {
        value: value,
        enumerable: true,
        configurable: true,
        writable: true
      });
    } else {
      obj[key] = value;
    }

    return obj;
  }

  function _classCallCheck(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
      throw new TypeError("Cannot call a class as a function");
    }
  }

  var _createClass = function () {
    function defineProperties(target, props) {
      for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) descriptor.writable = true;
        Object.defineProperty(target, descriptor.key, descriptor);
      }
    }

    return function (Constructor, protoProps, staticProps) {
      if (protoProps) defineProperties(Constructor.prototype, protoProps);
      if (staticProps) defineProperties(Constructor, staticProps);
      return Constructor;
    };
  }();

  var MatchState = exports.MatchState = function () {
    function MatchState(caseName, fields) {
      _classCallCheck(this, MatchState);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(MatchState, [{
      key: _Symbol3.default.reflection,
      value: function () {
        var _cases;

        return {
          type: "Cricket.CricketEngine.MatchState",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: (_cases = {}, _defineProperty(_cases, "A'Completed", [_Innings.Innings]), _defineProperty(_cases, "A'MatchDrawn", [_Innings.Innings]), _defineProperty(_cases, "A'Ongoing", [_Innings.Innings]), _defineProperty(_cases, "AB'CompletedNoFollowOn", [_Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "AB'CompletedPossibleFollowOn", [_Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "AB'MatchDrawn", [_Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "AB'Ongoing", [_Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABA'Completed", [_Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABA'MatchDrawn", [_Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABA'Ongoing", [_Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABA'VictoryB", [_Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABAB'MatchDrawn", [_Innings.Innings, _Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABAB'MatchTied", [_Innings.Innings, _Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABAB'Ongoing", [_Innings.Innings, _Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABAB'VictoryA", [_Innings.Innings, _Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABAB'VictoryB", [_Innings.Innings, _Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABB'Completed", [_Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABB'MatchDrawn", [_Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABB'Ongoing", [_Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABB'VictoryA", [_Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABBA'MatchDrawn", [_Innings.Innings, _Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABBA'MatchTied", [_Innings.Innings, _Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABBA'Ongoing", [_Innings.Innings, _Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABBA'VictoryA", [_Innings.Innings, _Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "ABBA'VictoryB", [_Innings.Innings, _Innings.Innings, _Innings.Innings, _Innings.Innings]), _defineProperty(_cases, "Abandoned", []), _defineProperty(_cases, "NotStarted", []), _cases)
        };
      }
    }, {
      key: "Equals",
      value: function (other) {
        return (0, _Util.equalsUnions)(this, other);
      }
    }, {
      key: "CompareTo",
      value: function (other) {
        return (0, _Util.compareUnions)(this, other);
      }
    }]);

    return MatchState;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.MatchState", MatchState);

  var SummaryMatchState = exports.SummaryMatchState = function () {
    function SummaryMatchState(caseName, fields) {
      _classCallCheck(this, SummaryMatchState);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(SummaryMatchState, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.SummaryMatchState",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            AwaitingFollowOnDecision: [],
            BetweenInnings: [],
            InningsInProgress: [_Innings.SummaryInningsState],
            MatchCompleted: [],
            NotYetStarted: []
          }
        };
      }
    }, {
      key: "Equals",
      value: function (other) {
        return (0, _Util.equalsUnions)(this, other);
      }
    }, {
      key: "CompareTo",
      value: function (other) {
        return (0, _Util.compareUnions)(this, other);
      }
    }]);

    return SummaryMatchState;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.SummaryMatchState", SummaryMatchState);

  var MatchUpdate = exports.MatchUpdate = function () {
    function MatchUpdate(caseName, fields) {
      _classCallCheck(this, MatchUpdate);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(MatchUpdate, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.MatchUpdate",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            AbandonMatch: [],
            DeclineFollowOn: [],
            DrawMatch: [],
            EnforceFollowOn: [],
            StartMatch: [],
            StartNextInnings: [],
            UpdateInnings: [_Innings.InningsUpdate]
          }
        };
      }
    }, {
      key: "Equals",
      value: function (other) {
        return (0, _Util.equalsUnions)(this, other);
      }
    }, {
      key: "CompareTo",
      value: function (other) {
        return (0, _Util.compareUnions)(this, other);
      }
    }]);

    return MatchUpdate;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.MatchUpdate", MatchUpdate);

  var MatchStateModule = exports.MatchStateModule = function (__exports) {
    var startMatch = function startMatch(state) {
      if (state.Case === "NotStarted") {
        return new MatchState("A'Ongoing", [_Innings.InningsModule.create]);
      } else {
        throw new Error("Call to StartMatch in invalid state");
      }
    };

    var abandonMatch = function abandonMatch(state) {
      if (state.Case === "NotStarted") {
        return new MatchState("Abandoned", []);
      } else {
        throw new Error("Call to AbandonMatch in invalid state");
      }
    };

    var drawMatch = function drawMatch(state) {
      var $var20 = state.Case === "A'Ongoing" ? [0, state.Fields[0]] : state.Case === "A'Completed" ? [0, state.Fields[0]] : state.Case === "AB'Ongoing" ? [1, state.Fields[0], state.Fields[1]] : state.Case === "AB'CompletedNoFollowOn" ? [1, state.Fields[0], state.Fields[1]] : state.Case === "AB'CompletedPossibleFollowOn" ? [1, state.Fields[0], state.Fields[1]] : state.Case === "ABA'Ongoing" ? [2, state.Fields[0], state.Fields[2], state.Fields[1]] : state.Case === "ABA'Completed" ? [2, state.Fields[0], state.Fields[2], state.Fields[1]] : state.Case === "ABB'Ongoing" ? [3, state.Fields[0], state.Fields[1], state.Fields[2]] : state.Case === "ABB'Completed" ? [3, state.Fields[0], state.Fields[1], state.Fields[2]] : state.Case === "ABAB'Ongoing" ? [4] : state.Case === "ABBA'Ongoing" ? [5] : [6];

      switch ($var20[0]) {
        case 0:
          return new MatchState("A'MatchDrawn", [$var20[1]]);

        case 1:
          return new MatchState("AB'MatchDrawn", [$var20[1], $var20[2]]);

        case 2:
          return new MatchState("ABA'MatchDrawn", [$var20[1], $var20[3], $var20[2]]);

        case 3:
          return new MatchState("ABB'MatchDrawn", [$var20[1], $var20[2], $var20[3]]);

        case 4:
          return new MatchState("ABAB'MatchDrawn", [state.Fields[0], state.Fields[1], state.Fields[2], state.Fields[3]]);

        case 5:
          return new MatchState("ABBA'MatchDrawn", [state.Fields[0], state.Fields[1], state.Fields[2], state.Fields[3]]);

        case 6:
          throw new Error("Call to DrawMatch in invalid state");
      }
    };

    var startNextInnings = function startNextInnings(state) {
      if (state.Case === "A'Completed") {
        return new MatchState("AB'Ongoing", [state.Fields[0], _Innings.InningsModule.create]);
      } else if (state.Case === "AB'CompletedNoFollowOn") {
        return new MatchState("ABA'Ongoing", [state.Fields[0], state.Fields[1], _Innings.InningsModule.create]);
      } else if (state.Case === "ABA'Completed") {
        return new MatchState("ABAB'Ongoing", [state.Fields[0], state.Fields[1], state.Fields[2], _Innings.InningsModule.create]);
      } else if (state.Case === "ABB'Completed") {
        return new MatchState("ABBA'Ongoing", [state.Fields[0], state.Fields[1], state.Fields[2], _Innings.InningsModule.create]);
      } else {
        throw new Error("Call to StartNextInnings in invalid state");
      }
    };

    var enforceFollowOn = function enforceFollowOn(state) {
      if (state.Case === "AB'CompletedPossibleFollowOn") {
        return new MatchState("ABB'Ongoing", [state.Fields[0], state.Fields[1], _Innings.InningsModule.create]);
      } else {
        throw new Error("Call to EnforceFollowOn in invalid state");
      }
    };

    var declineFollowOn = function declineFollowOn(state) {
      if (state.Case === "AB'CompletedPossibleFollowOn") {
        return new MatchState("ABA'Ongoing", [state.Fields[0], state.Fields[1], _Innings.InningsModule.create]);
      } else {
        throw new Error("Call to DeclineFollowOn in invalid state");
      }
    };

    var updateInnings = function updateInnings(inningsUpdater, rules, state) {
      if (state.Case === "A'Ongoing") {
        var matchValue = _Innings.InningsModule.update(inningsUpdater, null, state.Fields[0]);

        var activePatternResult383 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue);

        if (activePatternResult383.Case === "Choice2Of2") {
          return new MatchState("A'Completed", [activePatternResult383.Fields[0]]);
        } else {
          return new MatchState("A'Ongoing", [activePatternResult383.Fields[0]]);
        }
      } else if (state.Case === "AB'Ongoing") {
        var matchValue_1 = _Innings.InningsModule.update(inningsUpdater, null, state.Fields[1]);

        var activePatternResult386 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_1);

        if (activePatternResult386.Case === "Choice2Of2") {
          if (state.Fields[0].GetRuns >= activePatternResult386.Fields[0].GetRuns + rules.FollowOnMargin) {
            return new MatchState("AB'CompletedPossibleFollowOn", [state.Fields[0], activePatternResult386.Fields[0]]);
          } else {
            var activePatternResult385 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_1);

            if (activePatternResult385.Case === "Choice2Of2") {
              return new MatchState("AB'CompletedNoFollowOn", [state.Fields[0], activePatternResult385.Fields[0]]);
            } else {
              throw new Error("E:\\Prog\\Visual Studio 2015\\Projects\\Cricket\\CricketEngine\\MatchState.fs", 97, 18);
            }
          }
        } else {
          return new MatchState("AB'Ongoing", [state.Fields[0], activePatternResult386.Fields[0]]);
        }
      } else if (state.Case === "ABA'Ongoing") {
        var matchValue_2 = _Innings.InningsModule.update(inningsUpdater, null, state.Fields[2]);

        var activePatternResult389 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_2);

        if (activePatternResult389.Case === "Choice2Of2") {
          if (state.Fields[1].GetRuns > state.Fields[0].GetRuns + activePatternResult389.Fields[0].GetRuns) {
            return new MatchState("ABA'VictoryB", [state.Fields[0], state.Fields[1], activePatternResult389.Fields[0]]);
          } else {
            var activePatternResult388 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_2);

            if (activePatternResult388.Case === "Choice2Of2") {
              return new MatchState("ABA'Completed", [state.Fields[0], state.Fields[1], activePatternResult388.Fields[0]]);
            } else {
              throw new Error("E:\\Prog\\Visual Studio 2015\\Projects\\Cricket\\CricketEngine\\MatchState.fs", 102, 18);
            }
          }
        } else {
          return new MatchState("ABA'Ongoing", [state.Fields[0], state.Fields[1], activePatternResult389.Fields[0]]);
        }
      } else if (state.Case === "ABB'Ongoing") {
        var matchValue_3 = _Innings.InningsModule.update(inningsUpdater, null, state.Fields[2]);

        var activePatternResult392 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_3);

        if (activePatternResult392.Case === "Choice2Of2") {
          if (state.Fields[0].GetRuns > state.Fields[1].GetRuns + activePatternResult392.Fields[0].GetRuns) {
            return new MatchState("ABB'VictoryA", [state.Fields[0], state.Fields[1], activePatternResult392.Fields[0]]);
          } else {
            var activePatternResult391 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_3);

            if (activePatternResult391.Case === "Choice2Of2") {
              return new MatchState("ABB'Completed", [state.Fields[0], state.Fields[1], activePatternResult391.Fields[0]]);
            } else {
              throw new Error("E:\\Prog\\Visual Studio 2015\\Projects\\Cricket\\CricketEngine\\MatchState.fs", 107, 18);
            }
          }
        } else {
          return new MatchState("ABB'Ongoing", [state.Fields[0], state.Fields[1], activePatternResult392.Fields[0]]);
        }
      } else if (state.Case === "ABAB'Ongoing") {
        var toWin = 1 + state.Fields[0].GetRuns + state.Fields[2].GetRuns - state.Fields[1].GetRuns - state.Fields[3].GetRuns;

        var matchValue_4 = _Innings.InningsModule.update(inningsUpdater, toWin, state.Fields[3]);

        var $var21 = void 0;

        var activePatternResult397 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_4);

        if (activePatternResult397.Case === "Choice1Of2") {
          if (state.Fields[1].GetRuns + activePatternResult397.Fields[0].GetRuns > state.Fields[0].GetRuns + state.Fields[2].GetRuns) {
            $var21 = [0, activePatternResult397.Fields[0]];
          } else {
            $var21 = [1];
          }
        } else {
          $var21 = [1];
        }

        switch ($var21[0]) {
          case 0:
            return new MatchState("ABAB'VictoryB", [state.Fields[0], state.Fields[1], state.Fields[2], $var21[1]]);

          case 1:
            var activePatternResult396 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_4);

            if (activePatternResult396.Case === "Choice2Of2") {
              if (state.Fields[0].GetRuns + state.Fields[2].GetRuns > state.Fields[1].GetRuns + activePatternResult396.Fields[0].GetRuns) {
                return new MatchState("ABAB'VictoryA", [state.Fields[0], state.Fields[1], state.Fields[2], activePatternResult396.Fields[0]]);
              } else {
                var $var22 = void 0;

                var activePatternResult395 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_4);

                if (activePatternResult395.Case === "Choice2Of2") {
                  if (state.Fields[0].GetRuns + state.Fields[2].GetRuns === state.Fields[1].GetRuns + activePatternResult395.Fields[0].GetRuns) {
                    $var22 = [0, activePatternResult395.Fields[0]];
                  } else {
                    $var22 = [1];
                  }
                } else {
                  $var22 = [1];
                }

                switch ($var22[0]) {
                  case 0:
                    return new MatchState("ABAB'MatchTied", [state.Fields[0], state.Fields[1], state.Fields[2], $var22[1]]);

                  case 1:
                    var activePatternResult394 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_4);

                    if (activePatternResult394.Case === "Choice2Of2") {
                      throw new Error("Call to UpdateInnings in inconsistent state");
                    } else {
                      throw new Error("E:\\Prog\\Visual Studio 2015\\Projects\\Cricket\\CricketEngine\\MatchState.fs", 113, 18);
                    }

                }
              }
            } else {
              return new MatchState("ABAB'Ongoing", [state.Fields[0], state.Fields[1], state.Fields[2], activePatternResult396.Fields[0]]);
            }

        }
      } else if (state.Case === "ABBA'Ongoing") {
        var toWin_1 = 1 + state.Fields[1].GetRuns + state.Fields[2].GetRuns - state.Fields[0].GetRuns - state.Fields[3].GetRuns;

        var matchValue_5 = _Innings.InningsModule.update(inningsUpdater, toWin_1, state.Fields[3]);

        var $var23 = void 0;

        var activePatternResult402 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_5);

        if (activePatternResult402.Case === "Choice1Of2") {
          if (state.Fields[0].GetRuns + activePatternResult402.Fields[0].GetRuns > state.Fields[1].GetRuns + state.Fields[2].GetRuns) {
            $var23 = [0, activePatternResult402.Fields[0]];
          } else {
            $var23 = [1];
          }
        } else {
          $var23 = [1];
        }

        switch ($var23[0]) {
          case 0:
            return new MatchState("ABBA'VictoryA", [state.Fields[0], state.Fields[1], state.Fields[2], $var23[1]]);

          case 1:
            var activePatternResult401 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_5);

            if (activePatternResult401.Case === "Choice2Of2") {
              if (state.Fields[1].GetRuns + state.Fields[2].GetRuns > state.Fields[0].GetRuns + activePatternResult401.Fields[0].GetRuns) {
                return new MatchState("ABBA'VictoryB", [state.Fields[0], state.Fields[1], state.Fields[2], activePatternResult401.Fields[0]]);
              } else {
                var $var24 = void 0;

                var activePatternResult400 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_5);

                if (activePatternResult400.Case === "Choice2Of2") {
                  if (state.Fields[1].GetRuns + state.Fields[2].GetRuns === state.Fields[0].GetRuns + activePatternResult400.Fields[0].GetRuns) {
                    $var24 = [0, activePatternResult400.Fields[0]];
                  } else {
                    $var24 = [1];
                  }
                } else {
                  $var24 = [1];
                }

                switch ($var24[0]) {
                  case 0:
                    return new MatchState("ABBA'MatchTied", [state.Fields[0], state.Fields[1], state.Fields[2], $var24[1]]);

                  case 1:
                    var activePatternResult399 = _Innings.InningsModule["|InningsOngoing|InningsCompleted|"](matchValue_5);

                    if (activePatternResult399.Case === "Choice2Of2") {
                      throw new Error("Call to UpdateInnings in inconsistent state");
                    } else {
                      throw new Error("E:\\Prog\\Visual Studio 2015\\Projects\\Cricket\\CricketEngine\\MatchState.fs", 121, 18);
                    }

                }
              }
            } else {
              return new MatchState("ABBA'Ongoing", [state.Fields[0], state.Fields[1], state.Fields[2], activePatternResult401.Fields[0]]);
            }

        }
      } else {
        throw new Error("Call to UpdateInnings in invalid state");
      }
    };

    var update = __exports.update = function (rules, transition, state) {
      if (transition.Case === "AbandonMatch") {
        return abandonMatch(state);
      } else if (transition.Case === "DrawMatch") {
        return drawMatch(state);
      } else if (transition.Case === "StartNextInnings") {
        return startNextInnings(state);
      } else if (transition.Case === "EnforceFollowOn") {
        return enforceFollowOn(state);
      } else if (transition.Case === "DeclineFollowOn") {
        return declineFollowOn(state);
      } else if (transition.Case === "UpdateInnings") {
        return updateInnings(transition.Fields[0], rules, state);
      } else {
        return startMatch(state);
      }
    };

    var summaryState = __exports.summaryState = function (state) {
      var $var25 = state.Case === "Abandoned" ? [1] : state.Case === "A'MatchDrawn" ? [1] : state.Case === "AB'MatchDrawn" ? [1] : state.Case === "ABBA'VictoryA" ? [1] : state.Case === "ABB'VictoryA" ? [1] : state.Case === "ABA'MatchDrawn" ? [1] : state.Case === "ABA'VictoryB" ? [1] : state.Case === "ABBA'VictoryB" ? [1] : state.Case === "ABBA'MatchDrawn" ? [1] : state.Case === "ABBA'MatchTied" ? [1] : state.Case === "ABAB'MatchDrawn" ? [1] : state.Case === "ABAB'MatchTied" ? [1] : state.Case === "ABAB'VictoryB" ? [1] : state.Case === "ABAB'VictoryA" ? [1] : state.Case === "ABB'MatchDrawn" ? [1] : state.Case === "AB'CompletedPossibleFollowOn" ? [2] : state.Case === "A'Ongoing" ? [3] : state.Case === "AB'Ongoing" ? [4] : state.Case === "ABA'Ongoing" ? [5] : state.Case === "ABB'Ongoing" ? [6] : state.Case === "ABAB'Ongoing" ? [7] : state.Case === "ABBA'Ongoing" ? [8] : state.Case === "A'Completed" ? [9] : state.Case === "AB'CompletedNoFollowOn" ? [9] : state.Case === "ABA'Completed" ? [9] : state.Case === "ABB'Completed" ? [9] : [0];

      switch ($var25[0]) {
        case 0:
          return new SummaryMatchState("NotYetStarted", []);

        case 1:
          return new SummaryMatchState("MatchCompleted", []);

        case 2:
          return new SummaryMatchState("AwaitingFollowOnDecision", []);

        case 3:
          return new SummaryMatchState("InningsInProgress", [_Innings.InningsModule.summaryState(state.Fields[0])]);

        case 4:
          return new SummaryMatchState("InningsInProgress", [_Innings.InningsModule.summaryState(state.Fields[1])]);

        case 5:
          return new SummaryMatchState("InningsInProgress", [_Innings.InningsModule.summaryState(state.Fields[2])]);

        case 6:
          return new SummaryMatchState("InningsInProgress", [_Innings.InningsModule.summaryState(state.Fields[2])]);

        case 7:
          return new SummaryMatchState("InningsInProgress", [_Innings.InningsModule.summaryState(state.Fields[3])]);

        case 8:
          return new SummaryMatchState("InningsInProgress", [_Innings.InningsModule.summaryState(state.Fields[3])]);

        case 9:
          return new SummaryMatchState("BetweenInnings", []);
      }
    };

    var currentInnings = __exports.currentInnings = function (state) {
      var $var26 = state.Case === "Abandoned" ? [0] : state.Case === "A'Completed" ? [0] : state.Case === "A'MatchDrawn" ? [0] : state.Case === "AB'CompletedNoFollowOn" ? [0] : state.Case === "AB'CompletedPossibleFollowOn" ? [1] : state.Case === "AB'MatchDrawn" ? [1] : state.Case === "ABA'VictoryB" ? [1] : state.Case === "ABA'Completed" ? [2] : state.Case === "ABA'MatchDrawn" ? [2] : state.Case === "ABB'VictoryA" ? [2] : state.Case === "ABB'Completed" ? [3] : state.Case === "ABB'MatchDrawn" ? [3] : state.Case === "ABAB'VictoryA" ? [3] : state.Case === "ABAB'VictoryB" ? [4] : state.Case === "ABAB'MatchDrawn" ? [4] : state.Case === "ABAB'MatchTied" ? [4] : state.Case === "ABBA'VictoryA" ? [5] : state.Case === "ABBA'VictoryB" ? [5] : state.Case === "ABBA'MatchDrawn" ? [5] : state.Case === "ABBA'MatchTied" ? [6] : state.Case === "A'Ongoing" ? [7] : state.Case === "AB'Ongoing" ? [8] : state.Case === "ABA'Ongoing" ? [9, state.Fields[2]] : state.Case === "ABBA'Ongoing" ? [9, state.Fields[3]] : state.Case === "ABB'Ongoing" ? [10, state.Fields[2]] : state.Case === "ABAB'Ongoing" ? [10, state.Fields[3]] : [0];

      switch ($var26[0]) {
        case 0:
          throw new Error("no current innings");

        case 1:
          throw new Error("no current innings");

        case 2:
          throw new Error("no current innings");

        case 3:
          throw new Error("no current innings");

        case 4:
          throw new Error("no current innings");

        case 5:
          throw new Error("no current innings");

        case 6:
          throw new Error("no current innings");

        case 7:
          return state.Fields[0];

        case 8:
          return state.Fields[1];

        case 9:
          return $var26[1];

        case 10:
          return $var26[1];
      }
    };

    var currentBattingTeam = __exports.currentBattingTeam = function (state) {
      var $var27 = state.Case === "Abandoned" ? [0] : state.Case === "A'Completed" ? [0] : state.Case === "A'MatchDrawn" ? [0] : state.Case === "AB'CompletedNoFollowOn" ? [0] : state.Case === "AB'CompletedPossibleFollowOn" ? [1] : state.Case === "AB'MatchDrawn" ? [1] : state.Case === "ABA'VictoryB" ? [1] : state.Case === "ABA'Completed" ? [2] : state.Case === "ABA'MatchDrawn" ? [2] : state.Case === "ABB'VictoryA" ? [2] : state.Case === "ABB'Completed" ? [3] : state.Case === "ABB'MatchDrawn" ? [3] : state.Case === "ABAB'VictoryA" ? [3] : state.Case === "ABAB'VictoryB" ? [4] : state.Case === "ABAB'MatchDrawn" ? [4] : state.Case === "ABAB'MatchTied" ? [4] : state.Case === "ABBA'VictoryA" ? [5] : state.Case === "ABBA'VictoryB" ? [5] : state.Case === "ABBA'MatchDrawn" ? [5] : state.Case === "ABBA'MatchTied" ? [6] : state.Case === "A'Ongoing" ? [7] : state.Case === "ABA'Ongoing" ? [7] : state.Case === "ABBA'Ongoing" ? [7] : state.Case === "AB'Ongoing" ? [8] : state.Case === "ABB'Ongoing" ? [8] : state.Case === "ABAB'Ongoing" ? [8] : [0];

      switch ($var27[0]) {
        case 0:
          throw new Error("no current innings");

        case 1:
          throw new Error("no current innings");

        case 2:
          throw new Error("no current innings");

        case 3:
          throw new Error("no current innings");

        case 4:
          throw new Error("no current innings");

        case 5:
          throw new Error("no current innings");

        case 6:
          throw new Error("no current innings");

        case 7:
          return new _Types.Team("TeamA", []);

        case 8:
          return new _Types.Team("TeamB", []);
      }
    };

    var currentBowlingTeam = __exports.currentBowlingTeam = function (state) {
      var $var28 = state.Case === "Abandoned" ? [0] : state.Case === "A'Completed" ? [0] : state.Case === "A'MatchDrawn" ? [0] : state.Case === "AB'CompletedNoFollowOn" ? [0] : state.Case === "AB'CompletedPossibleFollowOn" ? [1] : state.Case === "AB'MatchDrawn" ? [1] : state.Case === "ABA'VictoryB" ? [1] : state.Case === "ABA'Completed" ? [2] : state.Case === "ABA'MatchDrawn" ? [2] : state.Case === "ABB'VictoryA" ? [2] : state.Case === "ABB'Completed" ? [3] : state.Case === "ABB'MatchDrawn" ? [3] : state.Case === "ABAB'VictoryA" ? [3] : state.Case === "ABAB'VictoryB" ? [4] : state.Case === "ABAB'MatchDrawn" ? [4] : state.Case === "ABAB'MatchTied" ? [4] : state.Case === "ABBA'VictoryA" ? [5] : state.Case === "ABBA'VictoryB" ? [5] : state.Case === "ABBA'MatchDrawn" ? [5] : state.Case === "ABBA'MatchTied" ? [6] : state.Case === "A'Ongoing" ? [7] : state.Case === "ABA'Ongoing" ? [7] : state.Case === "ABBA'Ongoing" ? [7] : state.Case === "AB'Ongoing" ? [8] : state.Case === "ABB'Ongoing" ? [8] : state.Case === "ABAB'Ongoing" ? [8] : [0];

      switch ($var28[0]) {
        case 0:
          throw new Error("no current innings");

        case 1:
          throw new Error("no current innings");

        case 2:
          throw new Error("no current innings");

        case 3:
          throw new Error("no current innings");

        case 4:
          throw new Error("no current innings");

        case 5:
          throw new Error("no current innings");

        case 6:
          throw new Error("no current innings");

        case 7:
          return new _Types.Team("TeamB", []);

        case 8:
          return new _Types.Team("TeamA", []);
      }
    };

    var totalRunsA = __exports.totalRunsA = function (state) {
      var $var29 = state.Case === "Abandoned" ? [0] : state.Case === "A'Ongoing" ? [1, state.Fields[0]] : state.Case === "A'Completed" ? [1, state.Fields[0]] : state.Case === "A'MatchDrawn" ? [1, state.Fields[0]] : state.Case === "AB'Ongoing" ? [2, state.Fields[0], state.Fields[1]] : state.Case === "AB'CompletedNoFollowOn" ? [2, state.Fields[0], state.Fields[1]] : state.Case === "AB'CompletedPossibleFollowOn" ? [2, state.Fields[0], state.Fields[1]] : state.Case === "AB'MatchDrawn" ? [2, state.Fields[0], state.Fields[1]] : state.Case === "ABA'Ongoing" ? [3, state.Fields[0], state.Fields[2], state.Fields[1]] : state.Case === "ABA'VictoryB" ? [3, state.Fields[0], state.Fields[2], state.Fields[1]] : state.Case === "ABA'Completed" ? [3, state.Fields[0], state.Fields[2], state.Fields[1]] : state.Case === "ABA'MatchDrawn" ? [3, state.Fields[0], state.Fields[2], state.Fields[1]] : state.Case === "ABB'Ongoing" ? [4, state.Fields[0], state.Fields[1], state.Fields[2]] : state.Case === "ABB'VictoryA" ? [4, state.Fields[0], state.Fields[1], state.Fields[2]] : state.Case === "ABB'Completed" ? [4, state.Fields[0], state.Fields[1], state.Fields[2]] : state.Case === "ABB'MatchDrawn" ? [4, state.Fields[0], state.Fields[1], state.Fields[2]] : state.Case === "ABAB'Ongoing" ? [5, state.Fields[0], state.Fields[2], state.Fields[1], state.Fields[3]] : state.Case === "ABAB'VictoryA" ? [5, state.Fields[0], state.Fields[2], state.Fields[1], state.Fields[3]] : state.Case === "ABAB'VictoryB" ? [5, state.Fields[0], state.Fields[2], state.Fields[1], state.Fields[3]] : state.Case === "ABAB'MatchDrawn" ? [5, state.Fields[0], state.Fields[2], state.Fields[1], state.Fields[3]] : state.Case === "ABAB'MatchTied" ? [5, state.Fields[0], state.Fields[2], state.Fields[1], state.Fields[3]] : state.Case === "ABBA'Ongoing" ? [5, state.Fields[0], state.Fields[3], state.Fields[1], state.Fields[2]] : state.Case === "ABBA'VictoryA" ? [5, state.Fields[0], state.Fields[3], state.Fields[1], state.Fields[2]] : state.Case === "ABBA'VictoryB" ? [5, state.Fields[0], state.Fields[3], state.Fields[1], state.Fields[2]] : state.Case === "ABBA'MatchDrawn" ? [5, state.Fields[0], state.Fields[3], state.Fields[1], state.Fields[2]] : state.Case === "ABBA'MatchTied" ? [5, state.Fields[0], state.Fields[3], state.Fields[1], state.Fields[2]] : [0];

      switch ($var29[0]) {
        case 0:
          return 0;

        case 1:
          return $var29[1].GetRuns;

        case 2:
          return $var29[1].GetRuns;

        case 3:
          return $var29[1].GetRuns + $var29[2].GetRuns;

        case 4:
          return $var29[1].GetRuns;

        case 5:
          return $var29[1].GetRuns + $var29[2].GetRuns;
      }
    };

    var totalRunsB = __exports.totalRunsB = function (state) {
      var $var30 = state.Case === "Abandoned" ? [0] : state.Case === "A'Ongoing" ? [1, state.Fields[0]] : state.Case === "A'Completed" ? [1, state.Fields[0]] : state.Case === "A'MatchDrawn" ? [1, state.Fields[0]] : state.Case === "AB'Ongoing" ? [2, state.Fields[0], state.Fields[1]] : state.Case === "AB'CompletedNoFollowOn" ? [2, state.Fields[0], state.Fields[1]] : state.Case === "AB'CompletedPossibleFollowOn" ? [2, state.Fields[0], state.Fields[1]] : state.Case === "AB'MatchDrawn" ? [2, state.Fields[0], state.Fields[1]] : state.Case === "ABA'Ongoing" ? [3, state.Fields[0], state.Fields[2], state.Fields[1]] : state.Case === "ABA'VictoryB" ? [3, state.Fields[0], state.Fields[2], state.Fields[1]] : state.Case === "ABA'Completed" ? [3, state.Fields[0], state.Fields[2], state.Fields[1]] : state.Case === "ABA'MatchDrawn" ? [3, state.Fields[0], state.Fields[2], state.Fields[1]] : state.Case === "ABB'Ongoing" ? [4, state.Fields[0], state.Fields[1], state.Fields[2]] : state.Case === "ABB'VictoryA" ? [4, state.Fields[0], state.Fields[1], state.Fields[2]] : state.Case === "ABB'Completed" ? [4, state.Fields[0], state.Fields[1], state.Fields[2]] : state.Case === "ABB'MatchDrawn" ? [4, state.Fields[0], state.Fields[1], state.Fields[2]] : state.Case === "ABAB'Ongoing" ? [5, state.Fields[0], state.Fields[2], state.Fields[1], state.Fields[3]] : state.Case === "ABAB'VictoryA" ? [5, state.Fields[0], state.Fields[2], state.Fields[1], state.Fields[3]] : state.Case === "ABAB'VictoryB" ? [5, state.Fields[0], state.Fields[2], state.Fields[1], state.Fields[3]] : state.Case === "ABAB'MatchDrawn" ? [5, state.Fields[0], state.Fields[2], state.Fields[1], state.Fields[3]] : state.Case === "ABAB'MatchTied" ? [5, state.Fields[0], state.Fields[2], state.Fields[1], state.Fields[3]] : state.Case === "ABBA'Ongoing" ? [5, state.Fields[0], state.Fields[3], state.Fields[1], state.Fields[2]] : state.Case === "ABBA'VictoryA" ? [5, state.Fields[0], state.Fields[3], state.Fields[1], state.Fields[2]] : state.Case === "ABBA'VictoryB" ? [5, state.Fields[0], state.Fields[3], state.Fields[1], state.Fields[2]] : state.Case === "ABBA'MatchDrawn" ? [5, state.Fields[0], state.Fields[3], state.Fields[1], state.Fields[2]] : state.Case === "ABBA'MatchTied" ? [5, state.Fields[0], state.Fields[3], state.Fields[1], state.Fields[2]] : [0];

      switch ($var30[0]) {
        case 0:
          return 0;

        case 1:
          return 0;

        case 2:
          return $var30[2].GetRuns;

        case 3:
          return $var30[3].GetRuns;

        case 4:
          return $var30[2].GetRuns + $var30[3].GetRuns;

        case 5:
          return $var30[3].GetRuns + $var30[4].GetRuns;
      }
    };

    var leadA = __exports.leadA = function (state) {
      return totalRunsA(state) - totalRunsB(state);
    };

    var leadB = __exports.leadB = function (state) {
      return totalRunsB(state) - totalRunsA(state);
    };

    return __exports;
  }({});
});