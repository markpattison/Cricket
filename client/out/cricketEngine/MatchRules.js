define(["exports", "fable-core/umd/Symbol", "fable-core/umd/Util"], function (exports, _Symbol2, _Util) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.MatchRules = undefined;

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

  var MatchRules = exports.MatchRules = function () {
    function MatchRules(followOnMargin) {
      _classCallCheck(this, MatchRules);

      this.FollowOnMargin = followOnMargin;
    }

    _createClass(MatchRules, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Cricket.CricketEngine.MatchRules",
          interfaces: ["FSharpRecord", "System.IEquatable", "System.IComparable"],
          properties: {
            FollowOnMargin: "number"
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

    return MatchRules;
  }();

  (0, _Symbol2.setType)("Cricket.CricketEngine.MatchRules", MatchRules);
});