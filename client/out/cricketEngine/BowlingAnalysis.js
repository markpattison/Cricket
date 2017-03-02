define(["exports", "fable-core/umd/Symbol", "fable-core/umd/Util", "./BallOutcome", "fable-core/umd/Seq"], function (exports, _Symbol2, _Util, _BallOutcome, _Seq) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.BowlingAnalysisModule = exports.BowlingAnalysis = undefined;

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

  var BowlingAnalysis = exports.BowlingAnalysis = function () {
    function BowlingAnalysis(balls, maidens, runsConceded, wickets) {
      _classCallCheck(this, BowlingAnalysis);

      this.Balls = balls;
      this.Maidens = maidens;
      this.RunsConceded = runsConceded;
      this.Wickets = wickets;
    }

    _createClass(BowlingAnalysis, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.BowlingAnalysis",
          interfaces: ["FSharpRecord", "System.IEquatable", "System.IComparable"],
          properties: {
            Balls: "number",
            Maidens: "number",
            RunsConceded: "number",
            Wickets: "number"
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

    return BowlingAnalysis;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.BowlingAnalysis", BowlingAnalysis);

  var BowlingAnalysisModule = exports.BowlingAnalysisModule = function (__exports) {
    var create = __exports.create = new BowlingAnalysis(0, 0, 0, 0);

    var update = __exports.update = function (ball, bowling) {
      return new BowlingAnalysis(bowling.Balls + 1, bowling.Maidens, bowling.RunsConceded + _BallOutcome.BallOutcomeModule.getRuns(ball), bowling.Wickets + (_BallOutcome.BallOutcomeModule.isWicketForBowler(ball) ? 1 : 0));
    };

    var updateAfterOver = __exports.updateAfterOver = function (balls, bowling) {
      var runsFromOver = function (list) {
        return (0, _Seq.sumBy)(function (ball) {
          return _BallOutcome.BallOutcomeModule.getRuns(ball);
        }, list);
      }(balls);

      if (runsFromOver > 0) {
        return bowling;
      } else {
        var Maidens = bowling.Maidens + 1;
        return new BowlingAnalysis(bowling.Balls, Maidens, bowling.RunsConceded, bowling.Wickets);
      }
    };

    return __exports;
  }({});
});