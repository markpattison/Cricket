define(["exports", "fable-core/umd/Symbol", "./MatchState", "./MatchRules", "fable-core/umd/Util", "fable-core/umd/Choice", "fable-core/umd/String", "./Formatting"], function (exports, _Symbol2, _MatchState, _MatchRules, _Util, _Choice, _String, _Formatting) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.MatchModule = exports.Match = undefined;

  var _Symbol3 = _interopRequireDefault(_Symbol2);

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

  var Match = exports.Match = function () {
    function Match(teamA, teamB, state, rules) {
      _classCallCheck(this, Match);

      this.TeamA = teamA;
      this.TeamB = teamB;
      this.State = state;
      this.Rules = rules;
    }

    _createClass(Match, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.Match",
          interfaces: ["FSharpRecord", "System.IEquatable", "System.IComparable"],
          properties: {
            TeamA: "string",
            TeamB: "string",
            State: _MatchState.MatchState,
            Rules: _MatchRules.MatchRules
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
    }]);

    return Match;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.Match", Match);

  var MatchModule = exports.MatchModule = function (__exports) {
    var updateMatchState = __exports.updateMatchState = function (update, match_) {
      var State = _MatchState.MatchStateModule.update(match_.Rules, update, match_.State);

      return new Match(match_.TeamA, match_.TeamB, State, match_.Rules);
    };

    var updateCurrentInnings = __exports.updateCurrentInnings = function (update) {
      var update_1 = new _MatchState.MatchUpdate("UpdateInnings", [update]);
      return function (match_) {
        return updateMatchState(update_1, match_);
      };
    };

    var newMatch = __exports.newMatch = function (rules, teamA, teamB) {
      return new Match(teamA, teamB, new _MatchState.MatchState("NotStarted", []), rules);
    };

    var _ALeads_ScoresLevel_BLeads_ = function _ALeads_ScoresLevel_BLeads_(state) {
      var matchValue = _MatchState.MatchStateModule.leadA(state);

      if (matchValue > 0) {
        return new _Choice2.default("Choice1Of3", [null]);
      } else if (matchValue < 0) {
        return new _Choice2.default("Choice3Of3", [null]);
      } else {
        return new _Choice2.default("Choice2Of3", [null]);
      }
    };

    var summaryStatus = __exports.summaryStatus = function (match_) {
      var leadA = _MatchState.MatchStateModule.leadA(match_.State);

      var leadB = _MatchState.MatchStateModule.leadB(match_.State);

      var $var31 = match_.State.Case === "NotStarted" ? [0] : match_.State.Case === "Abandoned" ? [1] : match_.State.Case === "A'MatchDrawn" ? [2] : match_.State.Case === "AB'MatchDrawn" ? [2] : match_.State.Case === "ABA'MatchDrawn" ? [2] : match_.State.Case === "ABB'MatchDrawn" ? [2] : match_.State.Case === "ABAB'MatchDrawn" ? [2] : match_.State.Case === "ABBA'MatchDrawn" ? [2] : match_.State.Case === "ABBA'MatchTied" ? [3] : match_.State.Case === "ABAB'MatchTied" ? [3] : match_.State.Case === "A'Ongoing" ? [4, match_.State.Fields[0]] : match_.State.Case === "A'Completed" ? match_.State.Fields[0].IsDeclared ? [5, match_.State.Fields[0]] : [6] : [6];

      switch ($var31[0]) {
        case 0:
          return "Match not started";

        case 1:
          return "Match abandoned without a ball being bowled";

        case 2:
          return "Match drawn";

        case 3:
          return "Match tied";

        case 4:
          return (0, _String.fsFormat)("%s are %i for %i in their first innings")(function (x) {
            return x;
          })(match_.TeamA)($var31[1].GetRuns)($var31[1].GetWickets);

        case 5:
          return (0, _String.fsFormat)("%s scored %i for %i declared in their first innings")(function (x) {
            return x;
          })(match_.TeamA)($var31[1].GetRuns)($var31[1].GetWickets);

        case 6:
          var $var32 = void 0;

          if (match_.State.Case === "A'Completed") {
            $var32 = [0, match_.State.Fields[0]];
          } else if (match_.State.Case === "ABA'VictoryB") {
            if (match_.State.Case === "AB'Ongoing") {
              var activePatternResult499 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult499.Case === "Choice2Of3") {
                $var32 = [2, match_.State.Fields[1]];
              } else if (activePatternResult499.Case === "Choice3Of3") {
                $var32 = [3, match_.State.Fields[1]];
              } else {
                $var32 = [1, match_.State.Fields[1]];
              }
            } else if (match_.State.Case === "AB'CompletedNoFollowOn") {
              var activePatternResult500 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult500.Case === "Choice2Of3") {
                $var32 = [5];
              } else if (activePatternResult500.Case === "Choice3Of3") {
                $var32 = [6];
              } else {
                $var32 = [4];
              }
            } else if (match_.State.Case === "AB'CompletedPossibleFollowOn") {
              var activePatternResult501 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult501.Case === "Choice2Of3") {
                $var32 = [8];
              } else if (activePatternResult501.Case === "Choice3Of3") {
                $var32 = [9];
              } else {
                $var32 = [7];
              }
            } else if (match_.State.Case === "ABA'Ongoing") {
              var activePatternResult502 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult502.Case === "Choice2Of3") {
                $var32 = [11, match_.State.Fields[2]];
              } else if (activePatternResult502.Case === "Choice1Of3") {
                $var32 = [12, match_.State.Fields[2]];
              } else {
                $var32 = [10, match_.State.Fields[2]];
              }
            } else if (match_.State.Case === "ABB'Ongoing") {
              $var32 = [13];
            } else {
              $var32 = [13];
            }
          } else if (match_.State.Case === "ABA'Completed") {
            if (match_.State.Case === "AB'Ongoing") {
              var activePatternResult503 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult503.Case === "Choice2Of3") {
                $var32 = [2, match_.State.Fields[1]];
              } else if (activePatternResult503.Case === "Choice3Of3") {
                $var32 = [3, match_.State.Fields[1]];
              } else {
                $var32 = [1, match_.State.Fields[1]];
              }
            } else if (match_.State.Case === "AB'CompletedNoFollowOn") {
              var activePatternResult504 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult504.Case === "Choice2Of3") {
                $var32 = [5];
              } else if (activePatternResult504.Case === "Choice3Of3") {
                $var32 = [6];
              } else {
                $var32 = [4];
              }
            } else if (match_.State.Case === "AB'CompletedPossibleFollowOn") {
              var activePatternResult505 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult505.Case === "Choice2Of3") {
                $var32 = [8];
              } else if (activePatternResult505.Case === "Choice3Of3") {
                $var32 = [9];
              } else {
                $var32 = [7];
              }
            } else if (match_.State.Case === "ABA'Ongoing") {
              var activePatternResult506 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult506.Case === "Choice2Of3") {
                $var32 = [11, match_.State.Fields[2]];
              } else if (activePatternResult506.Case === "Choice1Of3") {
                $var32 = [12, match_.State.Fields[2]];
              } else {
                $var32 = [10, match_.State.Fields[2]];
              }
            } else if (match_.State.Case === "ABB'Ongoing") {
              $var32 = [14];
            } else {
              $var32 = [14];
            }
          } else if (match_.State.Case === "ABB'VictoryA") {
            if (match_.State.Case === "AB'Ongoing") {
              var activePatternResult507 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult507.Case === "Choice2Of3") {
                $var32 = [2, match_.State.Fields[1]];
              } else if (activePatternResult507.Case === "Choice3Of3") {
                $var32 = [3, match_.State.Fields[1]];
              } else {
                $var32 = [1, match_.State.Fields[1]];
              }
            } else if (match_.State.Case === "AB'CompletedNoFollowOn") {
              var activePatternResult508 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult508.Case === "Choice2Of3") {
                $var32 = [5];
              } else if (activePatternResult508.Case === "Choice3Of3") {
                $var32 = [6];
              } else {
                $var32 = [4];
              }
            } else if (match_.State.Case === "AB'CompletedPossibleFollowOn") {
              var activePatternResult509 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult509.Case === "Choice2Of3") {
                $var32 = [8];
              } else if (activePatternResult509.Case === "Choice3Of3") {
                $var32 = [9];
              } else {
                $var32 = [7];
              }
            } else if (match_.State.Case === "ABA'Ongoing") {
              var activePatternResult510 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult510.Case === "Choice2Of3") {
                $var32 = [11, match_.State.Fields[2]];
              } else if (activePatternResult510.Case === "Choice1Of3") {
                $var32 = [12, match_.State.Fields[2]];
              } else {
                $var32 = [10, match_.State.Fields[2]];
              }
            } else if (match_.State.Case === "ABB'Ongoing") {
              var activePatternResult511 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult511.Case === "Choice2Of3") {
                $var32 = [16, match_.State.Fields[2]];
              } else if (activePatternResult511.Case === "Choice3Of3") {
                $var32 = [17, match_.State.Fields[2]];
              } else {
                $var32 = [15, match_.State.Fields[2]];
              }
            } else {
              $var32 = [18];
            }
          } else if (match_.State.Case === "ABB'Completed") {
            if (match_.State.Case === "AB'Ongoing") {
              var activePatternResult512 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult512.Case === "Choice2Of3") {
                $var32 = [2, match_.State.Fields[1]];
              } else if (activePatternResult512.Case === "Choice3Of3") {
                $var32 = [3, match_.State.Fields[1]];
              } else {
                $var32 = [1, match_.State.Fields[1]];
              }
            } else if (match_.State.Case === "AB'CompletedNoFollowOn") {
              var activePatternResult513 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult513.Case === "Choice2Of3") {
                $var32 = [5];
              } else if (activePatternResult513.Case === "Choice3Of3") {
                $var32 = [6];
              } else {
                $var32 = [4];
              }
            } else if (match_.State.Case === "AB'CompletedPossibleFollowOn") {
              var activePatternResult514 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult514.Case === "Choice2Of3") {
                $var32 = [8];
              } else if (activePatternResult514.Case === "Choice3Of3") {
                $var32 = [9];
              } else {
                $var32 = [7];
              }
            } else if (match_.State.Case === "ABA'Ongoing") {
              var activePatternResult515 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult515.Case === "Choice2Of3") {
                $var32 = [11, match_.State.Fields[2]];
              } else if (activePatternResult515.Case === "Choice1Of3") {
                $var32 = [12, match_.State.Fields[2]];
              } else {
                $var32 = [10, match_.State.Fields[2]];
              }
            } else if (match_.State.Case === "ABB'Ongoing") {
              var activePatternResult516 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult516.Case === "Choice2Of3") {
                $var32 = [16, match_.State.Fields[2]];
              } else if (activePatternResult516.Case === "Choice3Of3") {
                $var32 = [17, match_.State.Fields[2]];
              } else {
                $var32 = [15, match_.State.Fields[2]];
              }
            } else {
              $var32 = [19];
            }
          } else if (match_.State.Case === "ABAB'Ongoing") {
            if (match_.State.Case === "AB'Ongoing") {
              var activePatternResult517 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult517.Case === "Choice2Of3") {
                $var32 = [2, match_.State.Fields[1]];
              } else if (activePatternResult517.Case === "Choice3Of3") {
                $var32 = [3, match_.State.Fields[1]];
              } else {
                $var32 = [1, match_.State.Fields[1]];
              }
            } else if (match_.State.Case === "AB'CompletedNoFollowOn") {
              var activePatternResult518 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult518.Case === "Choice2Of3") {
                $var32 = [5];
              } else if (activePatternResult518.Case === "Choice3Of3") {
                $var32 = [6];
              } else {
                $var32 = [4];
              }
            } else if (match_.State.Case === "AB'CompletedPossibleFollowOn") {
              var activePatternResult519 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult519.Case === "Choice2Of3") {
                $var32 = [8];
              } else if (activePatternResult519.Case === "Choice3Of3") {
                $var32 = [9];
              } else {
                $var32 = [7];
              }
            } else if (match_.State.Case === "ABA'Ongoing") {
              var activePatternResult520 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult520.Case === "Choice2Of3") {
                $var32 = [11, match_.State.Fields[2]];
              } else if (activePatternResult520.Case === "Choice1Of3") {
                $var32 = [12, match_.State.Fields[2]];
              } else {
                $var32 = [10, match_.State.Fields[2]];
              }
            } else if (match_.State.Case === "ABB'Ongoing") {
              var activePatternResult521 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult521.Case === "Choice2Of3") {
                $var32 = [16, match_.State.Fields[2]];
              } else if (activePatternResult521.Case === "Choice3Of3") {
                $var32 = [17, match_.State.Fields[2]];
              } else {
                $var32 = [15, match_.State.Fields[2]];
              }
            } else {
              $var32 = [20, match_.State.Fields[3]];
            }
          } else if (match_.State.Case === "ABAB'VictoryA") {
            if (match_.State.Case === "AB'Ongoing") {
              var activePatternResult522 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult522.Case === "Choice2Of3") {
                $var32 = [2, match_.State.Fields[1]];
              } else if (activePatternResult522.Case === "Choice3Of3") {
                $var32 = [3, match_.State.Fields[1]];
              } else {
                $var32 = [1, match_.State.Fields[1]];
              }
            } else if (match_.State.Case === "AB'CompletedNoFollowOn") {
              var activePatternResult523 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult523.Case === "Choice2Of3") {
                $var32 = [5];
              } else if (activePatternResult523.Case === "Choice3Of3") {
                $var32 = [6];
              } else {
                $var32 = [4];
              }
            } else if (match_.State.Case === "AB'CompletedPossibleFollowOn") {
              var activePatternResult524 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult524.Case === "Choice2Of3") {
                $var32 = [8];
              } else if (activePatternResult524.Case === "Choice3Of3") {
                $var32 = [9];
              } else {
                $var32 = [7];
              }
            } else if (match_.State.Case === "ABA'Ongoing") {
              var activePatternResult525 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult525.Case === "Choice2Of3") {
                $var32 = [11, match_.State.Fields[2]];
              } else if (activePatternResult525.Case === "Choice1Of3") {
                $var32 = [12, match_.State.Fields[2]];
              } else {
                $var32 = [10, match_.State.Fields[2]];
              }
            } else if (match_.State.Case === "ABB'Ongoing") {
              var activePatternResult526 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult526.Case === "Choice2Of3") {
                $var32 = [16, match_.State.Fields[2]];
              } else if (activePatternResult526.Case === "Choice3Of3") {
                $var32 = [17, match_.State.Fields[2]];
              } else {
                $var32 = [15, match_.State.Fields[2]];
              }
            } else {
              $var32 = [21];
            }
          } else if (match_.State.Case === "ABAB'VictoryB") {
            if (match_.State.Case === "AB'Ongoing") {
              var activePatternResult527 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult527.Case === "Choice2Of3") {
                $var32 = [2, match_.State.Fields[1]];
              } else if (activePatternResult527.Case === "Choice3Of3") {
                $var32 = [3, match_.State.Fields[1]];
              } else {
                $var32 = [1, match_.State.Fields[1]];
              }
            } else if (match_.State.Case === "AB'CompletedNoFollowOn") {
              var activePatternResult528 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult528.Case === "Choice2Of3") {
                $var32 = [5];
              } else if (activePatternResult528.Case === "Choice3Of3") {
                $var32 = [6];
              } else {
                $var32 = [4];
              }
            } else if (match_.State.Case === "AB'CompletedPossibleFollowOn") {
              var activePatternResult529 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult529.Case === "Choice2Of3") {
                $var32 = [8];
              } else if (activePatternResult529.Case === "Choice3Of3") {
                $var32 = [9];
              } else {
                $var32 = [7];
              }
            } else if (match_.State.Case === "ABA'Ongoing") {
              var activePatternResult530 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult530.Case === "Choice2Of3") {
                $var32 = [11, match_.State.Fields[2]];
              } else if (activePatternResult530.Case === "Choice1Of3") {
                $var32 = [12, match_.State.Fields[2]];
              } else {
                $var32 = [10, match_.State.Fields[2]];
              }
            } else if (match_.State.Case === "ABB'Ongoing") {
              var activePatternResult531 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult531.Case === "Choice2Of3") {
                $var32 = [16, match_.State.Fields[2]];
              } else if (activePatternResult531.Case === "Choice3Of3") {
                $var32 = [17, match_.State.Fields[2]];
              } else {
                $var32 = [15, match_.State.Fields[2]];
              }
            } else {
              $var32 = [22, match_.State.Fields[3]];
            }
          } else if (match_.State.Case === "ABBA'Ongoing") {
            if (match_.State.Case === "AB'Ongoing") {
              var activePatternResult532 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult532.Case === "Choice2Of3") {
                $var32 = [2, match_.State.Fields[1]];
              } else if (activePatternResult532.Case === "Choice3Of3") {
                $var32 = [3, match_.State.Fields[1]];
              } else {
                $var32 = [1, match_.State.Fields[1]];
              }
            } else if (match_.State.Case === "AB'CompletedNoFollowOn") {
              var activePatternResult533 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult533.Case === "Choice2Of3") {
                $var32 = [5];
              } else if (activePatternResult533.Case === "Choice3Of3") {
                $var32 = [6];
              } else {
                $var32 = [4];
              }
            } else if (match_.State.Case === "AB'CompletedPossibleFollowOn") {
              var activePatternResult534 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult534.Case === "Choice2Of3") {
                $var32 = [8];
              } else if (activePatternResult534.Case === "Choice3Of3") {
                $var32 = [9];
              } else {
                $var32 = [7];
              }
            } else if (match_.State.Case === "ABA'Ongoing") {
              var activePatternResult535 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult535.Case === "Choice2Of3") {
                $var32 = [11, match_.State.Fields[2]];
              } else if (activePatternResult535.Case === "Choice1Of3") {
                $var32 = [12, match_.State.Fields[2]];
              } else {
                $var32 = [10, match_.State.Fields[2]];
              }
            } else if (match_.State.Case === "ABB'Ongoing") {
              var activePatternResult536 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult536.Case === "Choice2Of3") {
                $var32 = [16, match_.State.Fields[2]];
              } else if (activePatternResult536.Case === "Choice3Of3") {
                $var32 = [17, match_.State.Fields[2]];
              } else {
                $var32 = [15, match_.State.Fields[2]];
              }
            } else {
              $var32 = [23, match_.State.Fields[3]];
            }
          } else if (match_.State.Case === "ABBA'VictoryB") {
            if (match_.State.Case === "AB'Ongoing") {
              var activePatternResult537 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult537.Case === "Choice2Of3") {
                $var32 = [2, match_.State.Fields[1]];
              } else if (activePatternResult537.Case === "Choice3Of3") {
                $var32 = [3, match_.State.Fields[1]];
              } else {
                $var32 = [1, match_.State.Fields[1]];
              }
            } else if (match_.State.Case === "AB'CompletedNoFollowOn") {
              var activePatternResult538 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult538.Case === "Choice2Of3") {
                $var32 = [5];
              } else if (activePatternResult538.Case === "Choice3Of3") {
                $var32 = [6];
              } else {
                $var32 = [4];
              }
            } else if (match_.State.Case === "AB'CompletedPossibleFollowOn") {
              var activePatternResult539 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult539.Case === "Choice2Of3") {
                $var32 = [8];
              } else if (activePatternResult539.Case === "Choice3Of3") {
                $var32 = [9];
              } else {
                $var32 = [7];
              }
            } else if (match_.State.Case === "ABA'Ongoing") {
              var activePatternResult540 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult540.Case === "Choice2Of3") {
                $var32 = [11, match_.State.Fields[2]];
              } else if (activePatternResult540.Case === "Choice1Of3") {
                $var32 = [12, match_.State.Fields[2]];
              } else {
                $var32 = [10, match_.State.Fields[2]];
              }
            } else if (match_.State.Case === "ABB'Ongoing") {
              var activePatternResult541 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult541.Case === "Choice2Of3") {
                $var32 = [16, match_.State.Fields[2]];
              } else if (activePatternResult541.Case === "Choice3Of3") {
                $var32 = [17, match_.State.Fields[2]];
              } else {
                $var32 = [15, match_.State.Fields[2]];
              }
            } else {
              $var32 = [24];
            }
          } else if (match_.State.Case === "ABBA'VictoryA") {
            if (match_.State.Case === "AB'Ongoing") {
              var activePatternResult542 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult542.Case === "Choice2Of3") {
                $var32 = [2, match_.State.Fields[1]];
              } else if (activePatternResult542.Case === "Choice3Of3") {
                $var32 = [3, match_.State.Fields[1]];
              } else {
                $var32 = [1, match_.State.Fields[1]];
              }
            } else if (match_.State.Case === "AB'CompletedNoFollowOn") {
              var activePatternResult543 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult543.Case === "Choice2Of3") {
                $var32 = [5];
              } else if (activePatternResult543.Case === "Choice3Of3") {
                $var32 = [6];
              } else {
                $var32 = [4];
              }
            } else if (match_.State.Case === "AB'CompletedPossibleFollowOn") {
              var activePatternResult544 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult544.Case === "Choice2Of3") {
                $var32 = [8];
              } else if (activePatternResult544.Case === "Choice3Of3") {
                $var32 = [9];
              } else {
                $var32 = [7];
              }
            } else if (match_.State.Case === "ABA'Ongoing") {
              var activePatternResult545 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult545.Case === "Choice2Of3") {
                $var32 = [11, match_.State.Fields[2]];
              } else if (activePatternResult545.Case === "Choice1Of3") {
                $var32 = [12, match_.State.Fields[2]];
              } else {
                $var32 = [10, match_.State.Fields[2]];
              }
            } else if (match_.State.Case === "ABB'Ongoing") {
              var activePatternResult546 = _ALeads_ScoresLevel_BLeads_(match_.State);

              if (activePatternResult546.Case === "Choice2Of3") {
                $var32 = [16, match_.State.Fields[2]];
              } else if (activePatternResult546.Case === "Choice3Of3") {
                $var32 = [17, match_.State.Fields[2]];
              } else {
                $var32 = [15, match_.State.Fields[2]];
              }
            } else {
              $var32 = [25, match_.State.Fields[3]];
            }
          } else if (match_.State.Case === "AB'Ongoing") {
            var activePatternResult547 = _ALeads_ScoresLevel_BLeads_(match_.State);

            if (activePatternResult547.Case === "Choice2Of3") {
              $var32 = [2, match_.State.Fields[1]];
            } else if (activePatternResult547.Case === "Choice3Of3") {
              $var32 = [3, match_.State.Fields[1]];
            } else {
              $var32 = [1, match_.State.Fields[1]];
            }
          } else if (match_.State.Case === "AB'CompletedNoFollowOn") {
            var activePatternResult548 = _ALeads_ScoresLevel_BLeads_(match_.State);

            if (activePatternResult548.Case === "Choice2Of3") {
              $var32 = [5];
            } else if (activePatternResult548.Case === "Choice3Of3") {
              $var32 = [6];
            } else {
              $var32 = [4];
            }
          } else if (match_.State.Case === "AB'CompletedPossibleFollowOn") {
            var activePatternResult549 = _ALeads_ScoresLevel_BLeads_(match_.State);

            if (activePatternResult549.Case === "Choice2Of3") {
              $var32 = [8];
            } else if (activePatternResult549.Case === "Choice3Of3") {
              $var32 = [9];
            } else {
              $var32 = [7];
            }
          } else if (match_.State.Case === "ABA'Ongoing") {
            var activePatternResult550 = _ALeads_ScoresLevel_BLeads_(match_.State);

            if (activePatternResult550.Case === "Choice2Of3") {
              $var32 = [11, match_.State.Fields[2]];
            } else if (activePatternResult550.Case === "Choice1Of3") {
              $var32 = [12, match_.State.Fields[2]];
            } else {
              $var32 = [10, match_.State.Fields[2]];
            }
          } else if (match_.State.Case === "ABB'Ongoing") {
            var activePatternResult551 = _ALeads_ScoresLevel_BLeads_(match_.State);

            if (activePatternResult551.Case === "Choice2Of3") {
              $var32 = [16, match_.State.Fields[2]];
            } else if (activePatternResult551.Case === "Choice3Of3") {
              $var32 = [17, match_.State.Fields[2]];
            } else {
              $var32 = [15, match_.State.Fields[2]];
            }
          } else {
            $var32 = [26];
          }

          switch ($var32[0]) {
            case 0:
              return (0, _String.fsFormat)("%s scored %i all out in their first innings")(function (x) {
                return x;
              })(match_.TeamA)($var32[1].GetRuns);

            case 1:
              return (0, _String.fsFormat)("%s trail by %s with %s remaining in their first innings")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatRuns)(leadA))((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 2:
              return (0, _String.fsFormat)("%s are level with %s remaining in their first innings")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 3:
              return (0, _String.fsFormat)("%s lead by %s with %s remaining in their first innings")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatRuns)(leadB))((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 4:
              return (0, _String.fsFormat)("%s lead by %s after the first innings")(function (x) {
                return x;
              })(match_.TeamA)((0, _Formatting.formatRuns)(leadA));

            case 5:
              return (0, _String.fsFormat)("%s are level after the first innings")(function (x) {
                return x;
              })(match_.TeamB);

            case 6:
              return (0, _String.fsFormat)("%s lead by %s after the first innings")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatRuns)(leadB));

            case 7:
              return (0, _String.fsFormat)("%s lead by %s after the first innings")(function (x) {
                return x;
              })(match_.TeamA)((0, _Formatting.formatRuns)(leadA));

            case 8:
              return (0, _String.fsFormat)("%s are level after the first innings")(function (x) {
                return x;
              })(match_.TeamB);

            case 9:
              return (0, _String.fsFormat)("%s lead by %s after the first innings")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatRuns)(leadB));

            case 10:
              return (0, _String.fsFormat)("%s trail by %s with %s remaining in their second innings")(function (x) {
                return x;
              })(match_.TeamA)((0, _Formatting.formatRuns)(leadB))((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 11:
              return (0, _String.fsFormat)("%s are level with %s remaining in their second innings")(function (x) {
                return x;
              })(match_.TeamA)((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 12:
              return (0, _String.fsFormat)("%s lead by %s with %s remaining in their second innings")(function (x) {
                return x;
              })(match_.TeamA)((0, _Formatting.formatRuns)(leadA))((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 13:
              return (0, _String.fsFormat)("%s won by %s")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatRuns)(leadB));

            case 14:
              return (0, _String.fsFormat)("%s need %s to win in their second innings")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatRuns)(leadA + 1));

            case 15:
              return (0, _String.fsFormat)("%s trail by %s with %s remaining in their second innings")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatRuns)(leadA))((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 16:
              return (0, _String.fsFormat)("%s are level with %s remaining in their second innings")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 17:
              return (0, _String.fsFormat)("%s lead by %s with %s remaining in their second innings")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatRuns)(leadB))((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 18:
              return (0, _String.fsFormat)("%s won by an innings and %s")(function (x) {
                return x;
              })(match_.TeamA)((0, _Formatting.formatRuns)(leadA));

            case 19:
              return (0, _String.fsFormat)("%s need %s to win in their second innings")(function (x) {
                return x;
              })(match_.TeamA)((0, _Formatting.formatRuns)(leadB + 1));

            case 20:
              return (0, _String.fsFormat)("%s need %s to win with %s remaining")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatRuns)(leadA + 1))((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 21:
              return (0, _String.fsFormat)("%s won by %s")(function (x) {
                return x;
              })(match_.TeamA)((0, _Formatting.formatRuns)(leadA));

            case 22:
              return (0, _String.fsFormat)("%s won by %s")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 23:
              return (0, _String.fsFormat)("%s need %s to win with %s remaining")(function (x) {
                return x;
              })(match_.TeamA)((0, _Formatting.formatRuns)(leadB + 1))((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 24:
              return (0, _String.fsFormat)("%s won by %s")(function (x) {
                return x;
              })(match_.TeamB)((0, _Formatting.formatRuns)(leadB));

            case 25:
              return (0, _String.fsFormat)("%s won by %s")(function (x) {
                return x;
              })(match_.TeamA)((0, _Formatting.formatWicketsLeft)($var32[1].GetWickets));

            case 26:
              throw new Error("Unexpected match state");
          }

      }
    };

    return __exports;
  }({});
});