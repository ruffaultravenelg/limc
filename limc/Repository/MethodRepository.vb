Imports limc.TypeSystem

Namespace Repository
    Public Class MethodRepository

        ' Contains all methods with their C variants
        Private ReadOnly Methods As New Dictionary(Of String, IMethodVariantContainer)

        ' Register uncompiled function
        Public Sub RegisterUncompiledMethod(Method As AST.FunctionConstruct, AssociatedType As TypeSystem.Type)
            If Methods.ContainsKey(Method.Name) Then
                Throw New InternalError()
            End If
            Methods(Method.Name) = New UserMethodVariantContainer(Method, AssociatedType)
        End Sub
        Public Sub RegisterMethod(Method As Lazy.Method)
            If Methods.ContainsKey(Method.Name) Then
                Throw New InternalError()
            End If
            Methods(Method.Name) = New HardMethodVariantContainer(Method)
        End Sub

        ' General repository endpoint
        Public Function RetrieveMethod(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As Lazy.Method
            If Methods.ContainsKey(Name) Then
                Return Methods(Name).RetrieveWithGenerics(GenericTypes)
            Else
                Return Nothing
            End If
        End Function

        ' Represent a specific method, contains all C subfunctions
        Private Interface IMethodVariantContainer
            Function RetrieveWithGenerics(GenericTypes As IEnumerable(Of TypeSystem.Type)) As Lazy.Method
        End Interface

        ' Represent all variant of a user defined method
        Private Class UserMethodVariantContainer
            Implements IMethodVariantContainer

            Public ReadOnly Property Model As AST.FunctionConstruct
            Private AssociatedType As TypeSystem.Type

            Private LazyMethods As New List(Of Lazy.Method)

            Public Sub New(Model As AST.FunctionConstruct, AssociatedType As TypeSystem.Type)
                Me.Model = Model
                Me.AssociatedType = AssociatedType
            End Sub

            Public Function RetrieveWithGenerics(GenericTypes As IEnumerable(Of TypeSystem.Type)) As Lazy.Method Implements IMethodVariantContainer.RetrieveWithGenerics

                ' Count error
                If GenericTypes.Count <> Model.GenericArguments.Count Then
                    Throw New NotTheRightAmountOfGenericTypesException(GenericTypes.Count, Model.GenericArguments.Count)
                End If

                ' Search if there is a function already defined
                For Each Fn In LazyMethods
                    Return Fn 'TODO: handle generic types when there will be
                Next

                ' No function returned -> compile a new one
                Dim NewFn As New Lazy.UserMethod(AssociatedType, Model)
                LazyMethods.Add(NewFn)
                Return NewFn

            End Function

        End Class

        ' Represent a hard defined method
        Private Class HardMethodVariantContainer
            Implements IMethodVariantContainer

            Private Method As Lazy.Method

            Public Sub New(Method As Lazy.Method)
                Me.Method = Method
            End Sub

            Public Function RetrieveWithGenerics(GenericTypes As IEnumerable(Of Type)) As Lazy.Method Implements IMethodVariantContainer.RetrieveWithGenerics
                Return Method 'TODO: check for generic when there will be
            End Function

        End Class

    End Class
End Namespace