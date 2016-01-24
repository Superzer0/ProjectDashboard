(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('indexPluginsController', ['$scope', 'instancePluginsService', 'notificationService', indexPluginsController])
        .factory('instancePluginsService', ['$http', instancePluginsService]);

    function indexPluginsController($scope, instancePluginsService, notificationService) {
        $scope.plugins = [];

        $scope.reloadPlugins = function () {
            instancePluginsService.loadPlugins().success(function (data) {
                $scope.plugins = data;
            }).error(function (err) {
                console.log(err);
                notificationService.addNotification('instance plugins', 'error while loading plugins', 'error');
            });
        }

        $scope.roundPluginSize = function (size) {
            var number = new Number(size * 0.000001);
            return number.toFixed(2);
        }

        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }

        $scope.reloadPlugins();
    }

    function instancePluginsService($http) {
        var pluginsService = {};

        pluginsService.loadPlugins = function () {
            return $http.get('/api/instance/plugins/');
        }

        pluginsService.loadPlugin = function (pluginId, version) {
            return $http.get('/api/instance/plugins/get/' + pluginId + '/' + version);
        }

        pluginsService.loadActiveUserPlugins = function () {
            return $http.get('api/user/plugins/active/');
        }

        pluginsService.loadUserPlugins = function () {
            return $http.get('api/user/plugins/');
        }

        pluginsService.loadUserPluginConfiguration = function (pluginId, version) {
            return $http.get('api/user/plugins/get/' + pluginId + '/' + version);
        }

        return pluginsService;
    }

})();
