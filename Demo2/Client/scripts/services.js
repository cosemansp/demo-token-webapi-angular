(function (angular) {
    'use strict';

    angular.module('myApp')
        .service("authService", authService)
        .factory('Identity', identityFactory);

    function authService($q, $http, $window, $rootScope, $log, Identity, events) {

        this.authenticate = function (email, password, rememberMe) {
            $log.info("authenticate - user: " + email + ", passw:" + password);

            var deferred = $q.defer();
            $http.post('api/auth/login', { email: email, password: password })
                .then(function (response) {
                    var accessToken = response.data.accessToken;
                    $window.sessionStorage.setItem("token", accessToken);
                    if (rememberMe) {
                        // keep token in localStorage
                        $window.localStorage.setItem('token', accessToken);
                    }

                    // parse token to get claims
                    var claims = angular.fromJson($window.atob(accessToken.split('.')[1]));

                    // set identity 
                    var identity = Identity.build(claims);
                    $rootScope.identity = identity;

                    // notify others
                    $rootScope.$emit(events.userLoggedIn, identity);

                    // resolve promise as success
                    deferred.resolve(identity);
                })
                .catch(function (reason) {
                    $rootScope.identity = Identity.anonymous;

                    // reject promise
                    deferred.reject(reason);
                });

            return deferred.promise;
        }

        this.logout = function () {
            var identity = $rootScope.identity;

            // remove token from storage
            $window.sessionStorage.removeItem("token");
            $window.localStorage.removeItem("token");

            // set anonymous identity
            $rootScope.identity = Identity.anonymous;

            // log and notify to others
            $log.info("Identity logged out: " + identity.name);
            $rootScope.$emit(events.userLoggedOut, identity);

            return $q.when(identity);
        };

        this.init = function() {
            // set identity on startup
            var token = $window.sessionStorage.getItem('token');
            // todo check localstorage
            if (token != null) {
                var claims = angular.fromJson($window.atob(token.split('.')[1]));
                $rootScope.identity = Identity.build(claims);
                return $rootScope.identity;
            }
            return Identity.anonymous;
        }
    }

    function identityFactory() {

        var clazz = function (id, name, roles) {
            this.id = id;
            this.name = name;
            this.roles = roles; // array of roles
            this.authenticated = roles && roles.length > 0;
        }

        clazz.prototype.isInRole = function(role) {
            if (!this.authenticated)
                return false;

            return this.roles.indexOf(role) != -1;
        }

        clazz.prototype.isInAnyRole = function(roles) {
            if (!this.authenticated)
                return false;

            for (var i = 0; i < roles.length; i++) {
                if (this.isInRole(roles[i]))
                    return true;
            }
            return false;
        }

        clazz.prototype.isAuthenticated = function() {
            return this.authenticated;
        }

        clazz.possibleRoles = ['admin', 'editor', 'guest'];

        clazz.build = function(claims) {
            if (!claims) {
                return new clazz("0", "anonymous");
            }
            return new clazz(
                claims['userId'].split('/')[1],
                claims['name'],
                claims['roles'].split(',')
            );
        };

        clazz.anonymous = new clazz("0", "anonymous");

        return clazz;
    };

})(angular);