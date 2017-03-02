define(["exports", "fable-core/umd/Symbol", "fable-core/umd/Util", "fable-core/umd/List", "./Types", "./IndividualInnings", "./BowlingAnalysis", "./BallOutcome", "fable-core/umd/Seq", "fable-core/umd/Choice"], function (exports, _Symbol2, _Util, _List, _Types, _IndividualInnings, _BowlingAnalysis, _BallOutcome, _Seq, _Choice) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.InningsModule = exports.InningsUpdate = exports.SummaryInningsState = exports.Innings = exports.End = undefined;

  var _Symbol3 = _interopRequireDefault(_Symbol2);

  var _List2 = _interopRequireDefault(_List);

  var _Choice2 = _interopRequireDefault(_Choice);

  function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
      default: obj
    };
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

  var End = exports.End = function () {
    function End(caseName, fields) {
      _classCallCheck(this, End);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(End, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.End",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            End1: [],
            End2: []
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
    }, {
      key: "ToString",
      value: function () {
        if (this.Case === "End2") {
          return "end 2";
        } else {
          return "end 1";
        }
      }
    }, {
      key: "OtherEnd",
      get: function () {
        if ((0, _Util.equals)(this, new End("End1", []))) {
          return new End("End2", []);
        } else {
          return new End("End1", []);
        }
      }
    }]);

    return End;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.End", End);

  var Innings = exports.Innings = function () {
    function Innings(batsmen, bowlers, isDeclared, batsmanAtEnd1, batsmanAtEnd2, endFacingNext, oversCompleted, ballsThisOver, bowlerToEnd1, bowlerToEnd2) {
      _classCallCheck(this, Innings);

      this.Batsmen = batsmen;
      this.Bowlers = bowlers;
      this.IsDeclared = isDeclared;
      this.BatsmanAtEnd1 = batsmanAtEnd1;
      this.BatsmanAtEnd2 = batsmanAtEnd2;
      this.EndFacingNext = endFacingNext;
      this.OversCompleted = oversCompleted;
      this.BallsThisOver = ballsThisOver;
      this.BowlerToEnd1 = bowlerToEnd1;
      this.BowlerToEnd2 = bowlerToEnd2;
    }

    _createClass(Innings, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.Innings",
          interfaces: ["FSharpRecord", "System.IEquatable", "System.IComparable"],
          properties: {
            Batsmen: (0, _Util.makeGeneric)(_List2.default, {
              T: (0, _Util.Tuple)([_Types.Player, _IndividualInnings.IndividualInnings])
            }),
            Bowlers: (0, _Util.makeGeneric)(_List2.default, {
              T: (0, _Util.Tuple)([_Types.Player, _BowlingAnalysis.BowlingAnalysis])
            }),
            IsDeclared: "boolean",
            BatsmanAtEnd1: (0, _Util.Option)(_Types.Player),
            BatsmanAtEnd2: (0, _Util.Option)(_Types.Player),
            EndFacingNext: End,
            OversCompleted: "number",
            BallsThisOver: (0, _Util.makeGeneric)(_List2.default, {
              T: _BallOutcome.BallOutcome
            }),
            BowlerToEnd1: (0, _Util.Option)(_Types.Player),
            BowlerToEnd2: (0, _Util.Option)(_Types.Player)
          }
        };
      }
    }, {
      key: "Equals",
      value: function (other) {
        return (0, _Util.equalsRecords)(this, other);
      }
    }, {
      key: "CompareTo",
      value: function (other) {
        return (0, _Util.compareRecords)(this, other);
      }
    }, {
      key: "GetRuns",
      get: function () {
        return (0, _Seq.sumBy)(function (tupledArg) {
          return tupledArg[1].Score;
        }, this.Batsmen);
      }
    }, {
      key: "GetWickets",
      get: function () {
        return (0, _List.filter)(function (tupledArg) {
          return function () {
            return tupledArg[1].HowOut != null;
          }(null);
        }, this.Batsmen).length;
      }
    }, {
      key: "IsCompleted",
      get: function () {
        if (this.IsDeclared) {
          return true;
        } else {
          return this.GetWickets === 10;
        }
      }
    }, {
      key: "BallsSoFarThisOver",
      get: function () {
        return this.BallsThisOver.length;
      }
    }]);

    return Innings;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.Innings", Innings);

  var SummaryInningsState = exports.SummaryInningsState = function () {
    function SummaryInningsState(caseName, fields) {
      _classCallCheck(this, SummaryInningsState);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(SummaryInningsState, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.SummaryInningsState",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            BatsmanRequired: ["number"],
            BowlerRequiredTo: [End],
            Completed: [],
            EndOver: [],
            MidOver: []
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

    return SummaryInningsState;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.SummaryInningsState", SummaryInningsState);

  var InningsUpdate = exports.InningsUpdate = function () {
    function InningsUpdate(caseName, fields) {
      _classCallCheck(this, InningsUpdate);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(InningsUpdate, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.InningsUpdate",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            Declare: [],
            SendInBatsman: [_Types.Player],
            SendInBowler: [_Types.Player],
            UpdateForBall: [_BallOutcome.BallOutcome]
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

    return InningsUpdate;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.InningsUpdate", InningsUpdate);

  var InningsModule = exports.InningsModule = function (__exports) {
    var _InningsOngoing_InningsCompleted_ = __exports["|InningsOngoing|InningsCompleted|"] = function (innings) {
      if (innings.IsCompleted) {
        return new _Choice2.default("Choice2Of2", [innings]);
      } else {
        return new _Choice2.default("Choice1Of2", [innings]);
      }
    };

    var create = __exports.create = new Innings(new _List2.default(), new _List2.default(), false, null, null, new End("End1", []), 0, new _List2.default(), null, null);

    var forPlayer = __exports.forPlayer = function (player, state) {
      return (0, _Seq.find)(function (tupledArg) {
        return tupledArg[0].Equals(player);
      }, state.Batsmen)[1];
    };

    var forBowler = __exports.forBowler = function (player, state) {
      return (0, _Seq.find)(function (tupledArg) {
        return tupledArg[0].Equals(player);
      }, state.Bowlers)[1];
    };

    var updateBatsmen = function updateBatsmen(f, batsman, list) {
      return (0, _List.map)(function (tupledArg) {
        return [tupledArg[0], tupledArg[0].Equals(batsman) ? f(tupledArg[1]) : tupledArg[1]];
      }, list);
    };

    var tempBowler = new _Types.Player("testBowler");

    var addBowlerIfNeeded = function addBowlerIfNeeded(bowler, bowlers) {
      if ((0, _Seq.exists)(function (tupledArg) {
        return (0, _Util.equals)(tupledArg[0], bowler);
      }, bowlers)) {
        return bowlers;
      } else {
        return (0, _List.append)(bowlers, (0, _List.ofArray)([[bowler, _BowlingAnalysis.BowlingAnalysisModule.create]]));
      }
    };

    var updateBowlers = function updateBowlers(ball, bowler, list) {
      return (0, _List.map)(function (tupledArg) {
        return [tupledArg[0], tupledArg[0].Equals(bowler) ? _BowlingAnalysis.BowlingAnalysisModule.update(ball, tupledArg[1]) : tupledArg[1]];
      }, list);
    };

    var updateBowlersForEndOverIfNeeded = function updateBowlersForEndOverIfNeeded(overCompleted, ballsThisOver, bowler, list) {
      if (overCompleted) {
        return (0, _List.map)(function (tupledArg) {
          return [tupledArg[0], tupledArg[0].Equals(bowler) ? _BowlingAnalysis.BowlingAnalysisModule.updateAfterOver(ballsThisOver, tupledArg[1]) : tupledArg[1]];
        }, list);
      } else {
        return list;
      }
    };

    var updateForBall = function updateForBall(ballOutcome, state) {
      var swapEnds = _BallOutcome.BallOutcomeModule.changedEnds(ballOutcome);

      var countsAsBallFaced = _BallOutcome.BallOutcomeModule.countsAsBallFaced(ballOutcome);

      var overCompleted = countsAsBallFaced ? state.BallsSoFarThisOver === 5 : false;
      var patternInput = void 0;
      var matchValue = [state.EndFacingNext, state.BatsmanAtEnd1, state.BatsmanAtEnd2];
      var $var13 = matchValue[0].Case === "End2" ? matchValue[1] != null ? matchValue[2] != null ? [1, matchValue[1], matchValue[2]] : [2] : [2] : matchValue[1] != null ? matchValue[2] != null ? [0, matchValue[1], matchValue[2]] : [2] : [2];

      switch ($var13[0]) {
        case 0:
          patternInput = [$var13[1], $var13[2]];
          break;

        case 1:
          patternInput = [$var13[2], $var13[1]];
          break;

        case 2:
          throw new Error("cannot bowl ball without two batsmen");
          break;
      }

      var whoOut = _BallOutcome.BallOutcomeModule.whoOut(ballOutcome);

      var patternInput_1 = void 0;
      var matchValue_1 = [whoOut, swapEnds];

      if (matchValue_1[0].Case === "NonStriker") {
        if (matchValue_1[1]) {
          patternInput_1 = [null, patternInput[0]];
        } else {
          patternInput_1 = [patternInput[0], null];
        }
      } else if (matchValue_1[0].Case === "Nobody") {
        if (matchValue_1[1]) {
          patternInput_1 = [patternInput[1], patternInput[0]];
        } else {
          patternInput_1 = [patternInput[0], patternInput[1]];
        }
      } else if (matchValue_1[1]) {
        patternInput_1 = [patternInput[1], null];
      } else {
        patternInput_1 = [null, patternInput[1]];
      }

      var patternInput_2 = state.EndFacingNext.Case === "End2" ? [patternInput_1[1], patternInput_1[0]] : [patternInput_1[0], patternInput_1[1]];
      var bowler = void 0;
      var matchValue_2 = [state.EndFacingNext, state.BowlerToEnd1, state.BowlerToEnd2];
      var $var14 = matchValue_2[0].Case === "End2" ? matchValue_2[2] != null ? [1] : [2] : matchValue_2[1] != null ? [0] : [2];

      switch ($var14[0]) {
        case 0:
          var bowl = matchValue_2[1];
          bowler = bowl;
          break;

        case 1:
          var bowl_1 = matchValue_2[2];
          bowler = bowl_1;
          break;

        case 2:
          throw new Error("cannot bowl ball without bowler");
          break;
      }

      var ballsThisOver = (0, _List.append)(state.BallsThisOver, (0, _List.ofArray)([ballOutcome]));

      var Batsmen = function () {
        var f = function f(innings) {
          return _IndividualInnings.IndividualInningsModule.updateNonStriker(ballOutcome, innings);
        };

        return function (list) {
          return updateBatsmen(f, patternInput[1], list);
        };
      }()(function () {
        var f_1 = function f_1(innings_1) {
          return _IndividualInnings.IndividualInningsModule.update(tempBowler, ballOutcome, innings_1);
        };

        return function (list_1) {
          return updateBatsmen(f_1, patternInput[0], list_1);
        };
      }()(state.Batsmen));

      var Bowlers = function (list_2) {
        return updateBowlersForEndOverIfNeeded(overCompleted, ballsThisOver, bowler, list_2);
      }(function (list_3) {
        return updateBowlers(ballOutcome, bowler, list_3);
      }(function (bowlers) {
        return addBowlerIfNeeded(bowler, bowlers);
      }(state.Bowlers)));

      var EndFacingNext = overCompleted ? state.EndFacingNext.OtherEnd : state.EndFacingNext;
      var OversCompleted = overCompleted ? state.OversCompleted + 1 : state.OversCompleted;
      var BallsThisOver = overCompleted ? new _List2.default() : ballsThisOver;
      return new Innings(Batsmen, Bowlers, state.IsDeclared, patternInput_2[0], patternInput_2[1], EndFacingNext, OversCompleted, BallsThisOver, state.BowlerToEnd1, state.BowlerToEnd2);
    };

    var sendInBatsman = function sendInBatsman(nextBatsman, state) {
      var nextIndex = state.Batsmen.length;
      var matchValue = [state.BatsmanAtEnd1, state.BatsmanAtEnd2, nextIndex];

      if (matchValue[0] != null) {
        if (matchValue[1] == null) {
          var Batsmen = (0, _List.append)(state.Batsmen, (0, _List.ofArray)([[nextBatsman, _IndividualInnings.IndividualInningsModule.create]]));
          var BatsmanAtEnd2 = nextBatsman;
          return new Innings(Batsmen, state.Bowlers, state.IsDeclared, state.BatsmanAtEnd1, BatsmanAtEnd2, state.EndFacingNext, state.OversCompleted, state.BallsThisOver, state.BowlerToEnd1, state.BowlerToEnd2);
        } else {
          throw new Error("cannot send in new batsman unless one is out");
        }
      } else if (matchValue[1] != null) {
        var Batsmen_1 = (0, _List.append)(state.Batsmen, (0, _List.ofArray)([[nextBatsman, _IndividualInnings.IndividualInningsModule.create]]));
        var BatsmanAtEnd1 = nextBatsman;
        return new Innings(Batsmen_1, state.Bowlers, state.IsDeclared, BatsmanAtEnd1, state.BatsmanAtEnd2, state.EndFacingNext, state.OversCompleted, state.BallsThisOver, state.BowlerToEnd1, state.BowlerToEnd2);
      } else if (matchValue[2] === 0) {
        var Batsmen_2 = (0, _List.append)(state.Batsmen, (0, _List.ofArray)([[nextBatsman, _IndividualInnings.IndividualInningsModule.create]]));
        var BatsmanAtEnd1_1 = nextBatsman;
        return new Innings(Batsmen_2, state.Bowlers, state.IsDeclared, BatsmanAtEnd1_1, state.BatsmanAtEnd2, state.EndFacingNext, state.OversCompleted, state.BallsThisOver, state.BowlerToEnd1, state.BowlerToEnd2);
      } else {
        throw new Error("cannot have two batsmen out at the same time");
      }
    };

    var sendInBowler = function sendInBowler(nextBowler, state) {
      if (state.BallsSoFarThisOver !== 0) {
        throw new Error("cannot change bowler during an over");
      }

      var lastBowler = void 0;
      var matchValue = state.EndFacingNext.OtherEnd;

      if (matchValue.Case === "End2") {
        lastBowler = state.BowlerToEnd2;
      } else {
        lastBowler = state.BowlerToEnd1;
      }

      if (state.OversCompleted > 0 ? (0, _Util.equals)(lastBowler, nextBowler) : false) {
        throw new Error("bowler cannot bowl consecutive overs");
      }

      if (state.EndFacingNext.Case === "End2") {
        var BowlerToEnd2 = nextBowler;
        return new Innings(state.Batsmen, state.Bowlers, state.IsDeclared, state.BatsmanAtEnd1, state.BatsmanAtEnd2, state.EndFacingNext, state.OversCompleted, state.BallsThisOver, state.BowlerToEnd1, BowlerToEnd2);
      } else {
        var BowlerToEnd1 = nextBowler;
        return new Innings(state.Batsmen, state.Bowlers, state.IsDeclared, state.BatsmanAtEnd1, state.BatsmanAtEnd2, state.EndFacingNext, state.OversCompleted, state.BallsThisOver, BowlerToEnd1, state.BowlerToEnd2);
      }
    };

    var declare = function declare(state) {
      var IsDeclared = true;
      return new Innings(state.Batsmen, state.Bowlers, IsDeclared, state.BatsmanAtEnd1, state.BatsmanAtEnd2, state.EndFacingNext, state.OversCompleted, state.BallsThisOver, state.BowlerToEnd1, state.BowlerToEnd2);
    };

    var update = __exports.update = function (transition, toWin, state) {
      if (transition.Case === "SendInBatsman") {
        return sendInBatsman(transition.Fields[0], state);
      } else if (transition.Case === "SendInBowler") {
        return sendInBowler(transition.Fields[0], state);
      } else if (transition.Case === "UpdateForBall") {
        var ballRestricedForMatchEnd = _BallOutcome.BallOutcomeModule.restrictForEndMatch(toWin, transition.Fields[0]);

        return updateForBall(ballRestricedForMatchEnd, state);
      } else {
        return declare(state);
      }
    };

    var summaryState = __exports.summaryState = function (state) {
      if (state.IsCompleted) {
        return new SummaryInningsState("Completed", []);
      } else {
        var matchValue = [state.BatsmanAtEnd1, state.BatsmanAtEnd2];
        var $var15 = matchValue[0] != null ? matchValue[1] == null ? [1] : [2] : matchValue[1] != null ? [1] : [0];

        switch ($var15[0]) {
          case 0:
            return new SummaryInningsState("BatsmanRequired", [1]);

          case 1:
            return new SummaryInningsState("BatsmanRequired", [state.GetWickets + 2]);

          case 2:
            var matchValue_1 = [state.EndFacingNext, state.BowlerToEnd1, state.BowlerToEnd2];
            var $var16 = matchValue_1[0].Case === "End2" ? matchValue_1[2] == null ? [1] : [2] : matchValue_1[1] == null ? [0] : [2];

            switch ($var16[0]) {
              case 0:
                return new SummaryInningsState("BowlerRequiredTo", [new End("End1", [])]);

              case 1:
                return new SummaryInningsState("BowlerRequiredTo", [new End("End2", [])]);

              case 2:
                if (state.BallsSoFarThisOver === 0) {
                  return new SummaryInningsState("EndOver", []);
                } else {
                  return new SummaryInningsState("MidOver", []);
                }

            }

        }
      }
    };

    return __exports;
  }({});
});