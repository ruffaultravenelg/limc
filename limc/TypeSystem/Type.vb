Imports limc.Repository

Namespace TypeSystem

    Public MustInherit Class Type

        ' Main types
        Public Shared ReadOnly Property Int As IntType = New IntType()
        Public Shared ReadOnly Property Str As StrType = New StrType()
        Public Shared ReadOnly Property Bool As BoolType = New BoolType()
        Public Shared ReadOnly Property Float As FloatType = New FloatType()
        Shared Sub New()
            Int.Compile()
            Str.Compile()
            Bool.Compile()
            Float.Compile()
        End Sub

        ' C representation (ex: void*)
        Public MustOverride ReadOnly Property cRepresentation As String

        ' Is a pointer
        Public MustOverride ReadOnly Property IsPointer As Boolean

        ' Has pointer
        Public ReadOnly Property pointerCRepresentation As String
            Get
                If IsPointer Then
                    Return cRepresentation
                Else
                    Return cRepresentation & "*"
                End If
            End Get
        End Property

        ' Create a default value (ex: NULL)
        Public MustOverride Function DefaultValue(Scope As Context.Scope) As String

        ' Assgin value
        Public Overridable Sub SetVariableValue(Writer As CWriter, Variable As String, NewValue As String)
            Writer.WriteLine($"{Variable} = {NewValue};")
        End Sub

        ' Equality
        Public Shared Operator =(a As Type, b As Type) As Boolean
            If a Is Nothing AndAlso b Is Nothing Then
                Return True
            ElseIf a Is Nothing OrElse b Is Nothing Then
                Return False
            Else
                Return a.cRepresentation = b.cRepresentation
            End If
        End Operator
        Public Shared Operator <>(a As Type, b As Type) As Boolean
            Return Not a = b
        End Operator

        ' Relations
        Private ReadOnly Relations As New List(Of Lazy.Relation)
        Protected Sub RegisterRelation(Relation As Lazy.Relation)
            Relations.Add(Relation)
        End Sub
        Public Function GetRelation(Type As RelationType, ArgumentsTypes As IEnumerable(Of Type), Location As Location) As Lazy.Relation

            For Each Relation In Relations

                ' Check type
                If Relation.Type <> Type Then
                    Continue For
                End If

                ' Check arguments
                If ArgumentsTypes.Count <> Relation.ArgumentsTypes.Count Then
                    Continue For
                End If
                Dim AllGood As Boolean = True
                For i = 0 To ArgumentsTypes.Count - 1
                    If Relation.ArgumentsTypes(i) <> ArgumentsTypes(i) Then
                        AllGood = False
                        Exit For
                    End If
                Next

                If Not AllGood Then
                    Continue For
                End If

                Return Relation

            Next
            Throw New SyntaxError($"The ""{ToString()}"" type does not contain such a relation.", Location)
        End Function


        ' Getters
        Private Getters As New List(Of Lazy.Getter)
        Protected Sub RegisterGetter(Getter As Lazy.Getter)
            Getters.Add(Getter) 'TODO: check if method name already exist
        End Sub

        ' Setters
        Private Setters As New List(Of Lazy.Setter)
        Protected Sub RegisterSetter(Setter As Lazy.Setter)
            Setters.Add(Setter) 'TODO: check if method name already exist
        End Sub

        ' Method
        Private MethodRepository As New MethodRepository(AddressOf DefineStrMethod)
        Protected Sub RegisterMethod(Model As AST.FunctionConstruct)
            MethodRepository.RegisterMethod(Model, Me)
        End Sub
        Protected Sub RegisterMethod(Method As Lazy.Method)
            MethodRepository.RegisterMethod(Method)
        End Sub

        Protected Overridable Function DefineStrMethod() As Lazy.Method
            Return New Lazy.HardMethod(Me, "str", {}, Type.Str, {
                $"return ""{CodeGen.Helper.Sanitize(ToString())}"";"
            })
        End Function

        ' Search element (from inside of the type scope)
        Public Function RetrieveElementsFromInside(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As IEnumerable(Of SearchMatch)
            Dim Results As New List(Of SearchMatch)

            ' Search in getters
            For Each G In Getters
                If G.Name = Name Then
                    Results.Add(New SearchMatch(New ScopeGetter(G, Not IsPointer)))
                    Exit For
                End If
            Next

            ' Search in setters
            For Each S In Setters
                If S.Name = Name Then
                    Results.Add(New SearchMatch(New ScopeSetter(S, Not IsPointer)))
                    Exit For
                End If
            Next

            ' Search in methods
            'Results.AddRange(MethodRepository.RetrieveMethods(Name, GenericTypes).Select(Function(fn) New SearchMatch(fn)))

            Return Results
        End Function

        ' Search element (from external element, only show exported elements)
        Public Function RetrieveElementsFromOutside(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As IEnumerable(Of SearchMatch)
            Dim Results As New List(Of SearchMatch)

            ' Search in getters
            For Each G In Getters
                If G.Name = Name Then
                    Results.Add(New SearchMatch(G))
                    Exit For
                End If
            Next

            ' Search in setters
            For Each S In Setters
                If S.Name = Name Then
                    Results.Add(New SearchMatch(S))
                    Exit For
                End If
            Next

            ' Search in methods
            Results.AddRange(MethodRepository.RetrieveMethods(Name, GenericTypes).Select(Function(fn) New SearchMatch(fn)))

            Return Results
        End Function

    End Class

End Namespace