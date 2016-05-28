(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('userPluginsController', ['$scope', 'notificationService',
            'utilsService', 'instancePluginsService',
            userPluginsController]);

    function userPluginsController($scope, notificationService, utilsService, instancePluginsService) {

        $scope.plugins = null;

        $scope.loadUserPlugins = function () {
            instancePluginsService.loadUserPlugins()
                .success(function (data) {
                    if (!angular.equals([], data)) {
                        $scope.plugins = data;
                    }
                }).error(function (err) {
                    notificationService.addError('user plugins', 'error while loading user plugins list');
                    console.log(err);
                });
        }


        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }

        $scope.loadUserPlugins();
    }

    angular
        .module('angularAdmin')
        .controller('userPluginConfigurationController', ['$scope', 'notificationService',
            'utilsService', 'instancePluginsService', '$routeParams', '$timeout', '$location', '$route', userPluginConfigurationController]);

    function userPluginConfigurationController($scope, notificationService, utilsService,
        instancePluginsService, $routeParams, $timeout, $location, $route) {

        $scope.plugin = null;

        if (!($routeParams.id && $routeParams.version)) {
            $location.path('/user/index');
        }

        $scope.loadPlugin = function () {
            instancePluginsService.loadUserPluginConfiguration($routeParams.id, $routeParams.version)
                .success(function (data) {
                    if (!angular.equals([], data)) {
                        $scope.plugin = data;
                        $scope.pluginState = !data.disabled;
                        $scope.plugin.configuration = JSON.parse($scope.plugin.configuration);
                        $timeout(function () {
                            SyntaxHighlighter.highlight();
                        }, 0);
                    }

                }).error(function (err) {
                    notificationService.addError('active plugins', 'error while loading active plugins list');
                    console.log(err);
                });
        }

        $scope.onPluginStateChange = function (state) {
            var valueToChange = !state;
            instancePluginsService.setPluginStateForUser($scope.plugin.id, $scope.plugin.version, valueToChange)
                .success(function () {
                    $scope.plugin.disabled = valueToChange;
                }).error(function () {
                    notificationService.addError('plugin state', 'error while changing state for ' + $scope.plugin.name);
                    $scope.pluginState = !$scope.plugin.disabled;
                });
        };

        $scope.jsonEditorOptions = { mode: 'tree' };

        $scope.configureUserPlugin = function () {
            var stringifiedJson = JSON.stringify($scope.plugin.configuration);
            instancePluginsService.configureUserPlugin($scope.plugin.id, $scope.plugin.version, stringifiedJson)
                .success(function () {
                    $route.reload();
                }).error(function () {
                    notificationService.addError('plugin configuration', 'error while changing configuration for ' + $scope.plugin.name);
                });;
        };

        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }
        $scope.loadPlugin();
    }

})();
