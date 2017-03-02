define(["exports", "fable-core/umd/Symbol", "./Types", "fable-core/umd/Util", "fable-core/umd/String"], function (exports, _Symbol2, _Types, _Util, _String) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.BallOutcomeModule = exports.WhoOut = exports.BallOutcome = undefined;

  var _Symbol3 = _interopRequireDefault(_Symbol2);

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

  var BallOutcome = exports.BallOutcome = function () {
    function BallOutcome(caseName, fields) {
      _classCallCheck(this, BallOutcome);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(BallOutcome, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.BallOutcome",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            Bowled: [],
            Caught: [_Types.Player, "boolean"],
            DotBall: [],
            Four: [],
            HitWicket: [],
            LBW: [],
            RunOutNonStriker: ["number", "boolean"],
            RunOutStriker: ["number", "boolean"],
            ScoreRuns: ["number"],
            Six: [],
            Stumped: [_Types.Player]
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
        if (this.Case === "ScoreRuns") {
          return (0, _String.fsFormat)("run %i")(function (x) {
            return x;
          })(this.Fields[0]);
        } else if (this.Case === "Four") {
          return "four";
        } else if (this.Case === "Six") {
          return "six";
        } else if (this.Case === "Bowled") {
          return "out bowled";
        } else if (this.Case === "HitWicket") {
          return "out hit wicket";
        } else if (this.Case === "LBW") {
          return "out lbw";
        } else if (this.Case === "Caught") {
          return (0, _String.fsFormat)("out caught (%s)")(function (x) {
            return x;
          })(this.Fields[0].Name) + (this.Fields[1] ? ", batsmen crossed" : "");
        } else if (this.Case === "Stumped") {
          return (0, _String.fsFormat)("out stumped (%s)")(function (x) {
            return x;
          })(this.Fields[0].Name);
        } else if (this.Case === "RunOutStriker") {
          return (0, _String.fsFormat)("striker run out (%i runs)")(function (x) {
            return x;
          })(this.Fields[0]) + (this.Fields[1] ? ", batsmen crossed" : "");
        } else if (this.Case === "RunOutNonStriker") {
          return (0, _String.fsFormat)("non-striker run out (%i runs)")(function (x) {
            return x;
          })(this.Fields[0]) + (this.Fields[1] ? ", batsmen crossed" : "");
        } else {
          return "dot ball";
        }
      }
    }]);

    return BallOutcome;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.BallOutcome", BallOutcome);

  var WhoOut = exports.WhoOut = function () {
    function WhoOut(caseName, fields) {
      _classCallCheck(this, WhoOut);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(WhoOut, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.WhoOut",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            Nobody: [],
            NonStriker: [],
            Striker: []
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

    return WhoOut;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.WhoOut", WhoOut);

  var BallOutcomeModule = exports.BallOutcomeModule = function (__exports) {
    var getRuns = __exports.getRuns = function (ball) {
      var $var1 = ball.Case === "ScoreRuns" ? [1] : ball.Case === "Four" ? [2] : ball.Case === "Six" ? [3] : ball.Case === "RunOutStriker" ? [4] : ball.Case === "RunOutNonStriker" ? [5] : ball.Case === "Bowled" ? [6] : ball.Case === "HitWicket" ? [6] : ball.Case === "Caught" ? [6] : ball.Case === "LBW" ? [6] : ball.Case === "Stumped" ? [6] : [0];

      switch ($var1[0]) {
        case 0:
          return 0;

        case 1:
          return ball.Fields[0];

        case 2:
          return 4;

        case 3:
          return 6;

        case 4:
          return ball.Fields[0];

        case 5:
          return ball.Fields[0];

        case 6:
          return 0;
      }
    };

    var changedEnds = __exports.changedEnds = function (ball) {
      var $var2 = ball.Case === "DotBall" ? [0] : ball.Case === "Four" ? [0] : ball.Case === "Six" ? [0] : ball.Case === "Bowled" ? [0] : ball.Case === "HitWicket" ? [0] : ball.Case === "LBW" ? [0] : ball.Case === "Stumped" ? [0] : ball.Case === "ScoreRuns" ? ball.Fields[0] % 2 === 1 ? [1, ball.Fields[0]] : [2] : [2];

      switch ($var2[0]) {
        case 0:
          return false;

        case 1:
          return true;

        case 2:
          var $var3 = ball.Case === "ScoreRuns" ? [0] : ball.Case === "Caught" ? [1, ball.Fields[1]] : ball.Case === "RunOutStriker" ? ball.Fields[0] % 2 === 1 ? [2, ball.Fields[1], ball.Fields[0]] : [3] : [3];

          switch ($var3[0]) {
            case 0:
              return false;

            case 1:
              return $var3[1];

            case 2:
              return !$var3[1];

            case 3:
              var $var4 = ball.Case === "RunOutNonStriker" ? ball.Fields[0] % 2 === 1 ? [0, ball.Fields[1], ball.Fields[0]] : [1] : [1];

              switch ($var4[0]) {
                case 0:
                  return !$var4[1];

                case 1:
                  if (ball.Case === "RunOutStriker") {
                    return ball.Fields[1];
                  } else if (ball.Case === "RunOutNonStriker") {
                    return ball.Fields[1];
                  } else {
                    throw new Error("E:\\Prog\\Visual Studio 2015\\Projects\\Cricket\\CricketEngine\\BallOutcome.fs", 48, 14);
                  }

              }

          }

      }
    };

    var whoOut = __exports.whoOut = function (ball) {
      var $var5 = ball.Case === "LBW" ? [0] : ball.Case === "HitWicket" ? [0] : ball.Case === "Caught" ? [0] : ball.Case === "Stumped" ? [0] : ball.Case === "RunOutStriker" ? [0] : ball.Case === "RunOutNonStriker" ? [1] : ball.Case === "DotBall" ? [2] : ball.Case === "ScoreRuns" ? [2] : ball.Case === "Four" ? [2] : ball.Case === "Six" ? [2] : [0];

      switch ($var5[0]) {
        case 0:
          return new WhoOut("Striker", []);

        case 1:
          return new WhoOut("NonStriker", []);

        case 2:
          return new WhoOut("Nobody", []);
      }
    };

    var howStrikerOut = __exports.howStrikerOut = function (bowler, ball) {
      var $var6 = ball.Case === "LBW" ? [1] : ball.Case === "HitWicket" ? [2] : ball.Case === "Caught" ? [3] : ball.Case === "Stumped" ? [4] : ball.Case === "RunOutStriker" ? [5] : ball.Case === "RunOutNonStriker" ? [6] : ball.Case === "DotBall" ? [7] : ball.Case === "ScoreRuns" ? [7] : ball.Case === "Four" ? [7] : ball.Case === "Six" ? [7] : [0];

      switch ($var6[0]) {
        case 0:
          return new _Types.HowOut("Bowled", [bowler]);

        case 1:
          return new _Types.HowOut("LBW", [bowler]);

        case 2:
          return new _Types.HowOut("HitWicket", [bowler]);

        case 3:
          return new _Types.HowOut("Caught", [bowler, ball.Fields[0]]);

        case 4:
          return new _Types.HowOut("Stumped", [bowler, ball.Fields[0]]);

        case 5:
          return new _Types.HowOut("RunOut", []);

        case 6:
          return null;

        case 7:
          return null;
      }
    };

    var howNonStrikerOut = __exports.howNonStrikerOut = function (ball) {
      if (ball.Case === "RunOutNonStriker") {
        return new _Types.HowOut("RunOut", []);
      } else {
        return null;
      }
    };

    var countsAsBallFaced = __exports.countsAsBallFaced = function (ball) {
      return true;
    };

    var isAFour = __exports.isAFour = function (ball) {
      var $var7 = ball.Case === "Four" ? [0] : ball.Case === "ScoreRuns" ? ball.Fields[0] === 4 ? [0] : [1] : ball.Case === "RunOutStriker" ? ball.Fields[0] === 4 ? [0] : [1] : ball.Case === "RunOutNonStriker" ? ball.Fields[0] === 4 ? [0] : [1] : [1];

      switch ($var7[0]) {
        case 0:
          return true;

        case 1:
          return false;
      }
    };

    var isASix = __exports.isASix = function (ball) {
      var $var8 = ball.Case === "Six" ? [0] : ball.Case === "ScoreRuns" ? ball.Fields[0] === 6 ? [0] : [1] : ball.Case === "RunOutStriker" ? ball.Fields[0] === 6 ? [0] : [1] : ball.Case === "RunOutNonStriker" ? ball.Fields[0] === 6 ? [0] : [1] : [1];

      switch ($var8[0]) {
        case 0:
          return true;

        case 1:
          return false;
      }
    };

    var isWicketForBowler = __exports.isWicketForBowler = function (ball) {
      var $var9 = ball.Case === "Bowled" ? [0] : ball.Case === "HitWicket" ? [0] : ball.Case === "LBW" ? [0] : ball.Case === "Caught" ? [0] : ball.Case === "Stumped" ? [0] : [1];

      switch ($var9[0]) {
        case 0:
          return true;

        case 1:
          return false;
      }
    };

    var restrictForEndMatch = __exports.restrictForEndMatch = function (toWin, ball) {
      var matchValue = [toWin, ball];
      var $var10 = matchValue[0] != null ? matchValue[1].Case === "ScoreRuns" ? function () {
        var runsToWin_2 = matchValue[0];
        var runs_2 = matchValue[1].Fields[0];
        return runs_2 > runsToWin_2;
      }() ? [0, matchValue[1].Fields[0], matchValue[0]] : [1] : [1] : [1];

      switch ($var10[0]) {
        case 0:
          return new BallOutcome("ScoreRuns", [$var10[2]]);

        case 1:
          var $var11 = matchValue[0] != null ? matchValue[1].Case === "RunOutStriker" ? function () {
            var runsToWin_1 = matchValue[0];
            var runs_1 = matchValue[1].Fields[0];
            return runs_1 >= runsToWin_1;
          }() ? [0, matchValue[1].Fields[0], matchValue[0]] : [1] : [1] : [1];

          switch ($var11[0]) {
            case 0:
              return new BallOutcome("ScoreRuns", [$var11[2]]);

            case 1:
              var $var12 = matchValue[0] != null ? matchValue[1].Case === "RunOutNonStriker" ? function () {
                var runsToWin = matchValue[0];
                var runs = matchValue[1].Fields[0];
                return runs >= runsToWin;
              }() ? [0, matchValue[1].Fields[0], matchValue[0]] : [1] : [1] : [1];

              switch ($var12[0]) {
                case 0:
                  return new BallOutcome("ScoreRuns", [$var12[2]]);

                case 1:
                  return ball;
              }

          }

      }
    };

    return __exports;
  }({});
});