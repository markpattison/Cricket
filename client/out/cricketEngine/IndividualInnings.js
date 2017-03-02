define(["exports", "fable-core/umd/Symbol", "fable-core/umd/Util", "./Types", "./BallOutcome"], function (exports, _Symbol2, _Util, _Types, _BallOutcome) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.IndividualInningsModule = exports.IndividualInnings = undefined;

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

  var IndividualInnings = exports.IndividualInnings = function () {
    function IndividualInnings(score, howOut, ballsFaced, fours, sixes) {
      _classCallCheck(this, IndividualInnings);

      this.Score = score;
      this.HowOut = howOut;
      this.BallsFaced = ballsFaced;
      this.Fours = fours;
      this.Sixes = sixes;
    }

    _createClass(IndividualInnings, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.IndividualInnings",
          interfaces: ["FSharpRecord", "System.IEquatable", "System.IComparable"],
          properties: {
            Score: "number",
            HowOut: (0, _Util.Option)(_Types.HowOut),
            BallsFaced: "number",
            Fours: "number",
            Sixes: "number"
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

    return IndividualInnings;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.IndividualInnings", IndividualInnings);

  var IndividualInningsModule = exports.IndividualInningsModule = function (__exports) {
    var create = __exports.create = new IndividualInnings(0, null, 0, 0, 0);

    var update = __exports.update = function (bowler, ball, innings) {
      return new IndividualInnings(innings.Score + _BallOutcome.BallOutcomeModule.getRuns(ball), _BallOutcome.BallOutcomeModule.howStrikerOut(bowler, ball), innings.BallsFaced + (_BallOutcome.BallOutcomeModule.countsAsBallFaced(ball) ? 1 : 0), innings.Fours + (_BallOutcome.BallOutcomeModule.isAFour(ball) ? 1 : 0), innings.Sixes + (_BallOutcome.BallOutcomeModule.isASix(ball) ? 1 : 0));
    };

    var updateNonStriker = __exports.updateNonStriker = function (ball, innings) {
      if (ball.Case === "RunOutNonStriker") {
        var HowOut = new _Types.HowOut("RunOut", []);
        return new IndividualInnings(innings.Score, HowOut, innings.BallsFaced, innings.Fours, innings.Sixes);
      } else {
        return innings;
      }
    };

    return __exports;
  }({});
});