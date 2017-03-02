define(["exports", "fable-core/umd/Symbol", "fable-core/umd/Util", "fable-core/umd/String"], function (exports, _Symbol2, _Util, _String) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.Team = exports.HowOut = exports.Player = undefined;

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

  var Player = exports.Player = function () {
    function Player(name) {
      _classCallCheck(this, Player);

      this.Name = name;
    }

    _createClass(Player, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.Player",
          interfaces: ["FSharpRecord", "System.IEquatable", "System.IComparable"],
          properties: {
            Name: "string"
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

    return Player;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.Player", Player);

  var HowOut = exports.HowOut = function () {
    function HowOut(caseName, fields) {
      _classCallCheck(this, HowOut);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(HowOut, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.HowOut",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            Bowled: [Player],
            Caught: [Player, Player],
            HandledTheBall: [],
            HitTheBallTwice: [],
            HitWicket: [Player],
            LBW: [Player],
            ObstructingTheField: [],
            RunOut: [],
            Stumped: [Player, Player],
            TimedOut: []
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
        if (this.Case === "TimedOut") {
          return "timed out";
        } else if (this.Case === "Caught") {
          return (0, _String.fsFormat)("c %s b %s")(function (x) {
            return x;
          })(this.Fields[1].Name)(this.Fields[0].Name);
        } else if (this.Case === "HandledTheBall") {
          return "out handled the ball";
        } else if (this.Case === "HitTheBallTwice") {
          return "out hit the ball twice";
        } else if (this.Case === "HitWicket") {
          return (0, _String.fsFormat)("hit wicket b %s")(function (x) {
            return x;
          })(this.Fields[0].Name);
        } else if (this.Case === "LBW") {
          return (0, _String.fsFormat)("lbw b %s")(function (x) {
            return x;
          })(this.Fields[0].Name);
        } else if (this.Case === "ObstructingTheField") {
          return "out obstructing the field";
        } else if (this.Case === "RunOut") {
          return "run out";
        } else if (this.Case === "Stumped") {
          return (0, _String.fsFormat)("st %s b %s")(function (x) {
            return x;
          })(this.Fields[1].Name)(this.Fields[0].Name);
        } else {
          return (0, _String.fsFormat)("b %s")(function (x) {
            return x;
          })(this.Fields[0].Name);
        }
      }
    }]);

    return HowOut;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.HowOut", HowOut);

  var Team = exports.Team = function () {
    function Team(caseName, fields) {
      _classCallCheck(this, Team);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(Team, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.Team",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            TeamA: [],
            TeamB: []
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

    return Team;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.Team", Team);
});